using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiAll.Contex;
using ApiAll.Model.water;
using ApiAll.Model.tekistil;
using Newtonsoft.Json.Linq;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterOrdersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterOrdersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/WaterOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> GetWaterOrder()
        {
            return await _context.WaterOrder.ToListAsync();
        }

        public class CloseOrderDto
        {
            public long OrderId { get; set; }
            public long ClientId { get; set; }
            public long AddressId { get; set; }
            public long ProductId { get; set; }
            public double OlinganBakalshkaSoni { get; set; }
            public double BerilganBakalshkaSoni { get; set; }
            public long CheckId { get; set; }
            public WaterCheck Check { get; set; } // agar yangi check yuborish kerak bo‘lsa
        }

        [HttpPost("close-order")]
        public async Task<IActionResult> CloseOrder([FromBody] CloseOrderDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Zakazni qabul qilish (closeAcceptOrder)
                var waterOrder = await _context.WaterOrder.FindAsync(dto.OrderId);
                if (waterOrder == null) return NotFound("Order topilmadi");

                if (waterOrder.accepted_status == true)
                    return BadRequest("Order already accepted");

                waterOrder.accepted_status = true;
                _context.WaterOrder.Update(waterOrder);
                await _context.SaveChangesAsync();

                // 2. Bottle info (addBotlfInfoToClientForAccept)
                var bottleInfo = await _context.WaterClientBottleInfo
                    .FirstOrDefaultAsync(p => p.WaterClientid == dto.ClientId &&
                                            p.WaterClientAddressid == dto.AddressId &&
                                            p.WaterProductid == dto.ProductId);

                if (bottleInfo == null)
                {
                    bottleInfo = new WaterClientBottleInfo
                    {
                        bottle_count = dto.OlinganBakalshkaSoni + dto.BerilganBakalshkaSoni,
                        bottle_count_real = dto.BerilganBakalshkaSoni,
                        WaterProductid = dto.ProductId,
                        WaterClientAddressid = dto.AddressId,
                        WaterClientid = dto.ClientId,
                        active_status = true
                    };
                    _context.WaterClientBottleInfo.Add(bottleInfo);
                }
                else
                {
                    bottleInfo.bottle_count += dto.OlinganBakalshkaSoni + dto.BerilganBakalshkaSoni;
                    bottleInfo.bottle_count_real += dto.BerilganBakalshkaSoni;
                    _context.WaterClientBottleInfo.Update(bottleInfo);
                }
                await _context.SaveChangesAsync();

                // 3. Check (PostWaterCheck)
                if (dto.Check != null)
                {
                    _context.WaterCheck.Update(dto.Check);
                    await _context.SaveChangesAsync();
                }

                // 4. updateOrderFullReturningInfo
                waterOrder.reserverd_numeric_id_1 = -1 * dto.OlinganBakalshkaSoni;
                waterOrder.reserverd_numeric_id_2 = dto.BerilganBakalshkaSoni;
                waterOrder.reserverd_number_id_1 = bottleInfo.id; // yoki dto.CheckId
                waterOrder.reserverd_number_id_2 = dto.Check.id;
                _context.WaterOrder.Update(waterOrder);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, order = waterOrder, bottleInfo });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("getPaginationOrderByClientId")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationOrderByClientId([FromQuery] int page, [FromQuery] int size, [FromQuery] long client_id)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
               .Include(p => p.client)
               .Include(p => p.address)
               .Include(p => p.user)
               .Include(p => p.deleivered_user_auth)
                .ThenInclude(p => p.user)
                .Where(p => p.active_status == true && p.WaterClientid == client_id)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true && p.WaterClientid == client_id).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpGet("getOrderListByStrListWithAuthIdDelivered")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> getOrderListByStrListWithAuthIdDelivered([FromQuery] long id_auth)
        {
            var waterAuth = await _context.WaterAuth.FindAsync(id_auth);

            if (waterAuth == null)
            {
                return NotFound();
            }
            if (waterAuth.reserverd_note == null || waterAuth.reserverd_note.Trim().Length == 0)
            {
                return NotFound();
            }

            String[] id_list = waterAuth.reserverd_note.Split(',');
            List<WaterOrder> order_list = new List<WaterOrder>();
            foreach (String it in id_list)
            {
                try
                {
                    long id = long.Parse(it);
                    var waterOrder = await _context.WaterOrder
               .Include(p => p.client)
               .Include(p => p.address)
               .Include(p => p.user)
               .Where(p => p.id == id && p.accepted_status == true).ToListAsync();

                    if (waterOrder == null || waterOrder.Count == 0)
                    {
                        continue;
                    }

                    waterOrder.First().items = await _context.WaterOrderItem
                     .Include(p => p.product)
                     .Where(p => p.WaterOrderid == waterOrder.First().id)
                     .ToListAsync();
                    order_list.Add(waterOrder.First());
                }
                catch (FormatException) { }


            }

            ////add order list to phone list
            foreach (WaterOrder order_it in order_list)
            {
                order_it.phone_list_obj = await _context.WaterClientPhoneNumber.Where(p => p.WaterClientid == order_it.WaterClientid).ToListAsync();

            }


            return order_list;
        }

        [HttpGet("getOrderListByStrListWithAuthId")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> getOrderListByStrListWithAuthId([FromQuery] long id_auth)
        {
            var waterAuth = await _context.WaterAuth.FindAsync(id_auth);

            if (waterAuth == null)
            {
                return NotFound();
            }
            if (waterAuth.reserverd_note == null || waterAuth.reserverd_note.Trim().Length == 0) {
                return NotFound();
            }

            String[] id_list = waterAuth.reserverd_note.Split(',');
            List<WaterOrder> order_list = new List<WaterOrder>();
            foreach (String it in id_list)
            {
                try
                {
                    long id = long.Parse(it);
                    var waterOrder = await _context.WaterOrder
               .Include(p => p.client)
               .Include(p => p.address)
               .Include(p => p.user)
               .Where(p => p.id == id && p.accepted_status == false).ToListAsync();

                    if (waterOrder == null || waterOrder.Count == 0)
                    {
                        continue;
                    }

                    waterOrder.First().items = await _context.WaterOrderItem
                     .Include(p => p.product)
                     .Where(p => p.WaterOrderid == waterOrder.First().id)
                     .ToListAsync();
                    order_list.Add(waterOrder.First());
                }
                catch (FormatException) { }


            }

            ////add order list to phone list
            foreach (WaterOrder order_it in order_list) {
                order_it.phone_list_obj = await _context.WaterClientPhoneNumber.Where(p => p.WaterClientid == order_it.WaterClientid).ToListAsync();

            }


            return order_list;
        }

        [HttpGet("getTodayCompletedOrdersByAuthId")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> getTodayCompletedOrdersByAuthId([FromQuery] long id_auth)
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            var order_list = await _context.WaterOrder
                .Include(p => p.client)
                .Include(p => p.address)
                .Include(p => p.user)
                .Include(p => p.deleivered_user_auth)
                .ThenInclude(p => p.user)
                .Where(p => p.deleivered_user_auth_id == id_auth 
                    && p.accepted_status == true
                    && p.order_accepted_date >= today 
                    && p.order_accepted_date < tomorrow)
                .OrderByDescending(p => p.order_accepted_date)
                .ToListAsync();

            foreach (WaterOrder order in order_list)
            {
                order.items = await _context.WaterOrderItem
                    .Include(p => p.product)
                    .Where(p => p.WaterOrderid == order.id)
                    .ToListAsync();

                order.phone_list_obj = await _context.WaterClientPhoneNumber
                    .Where(p => p.WaterClientid == order.WaterClientid)
                    .ToListAsync();
            }

            return order_list;
        }

        [HttpGet("getOrderListByStrList")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> getOrderListByStrList([FromQuery] String id_str)
        {
            String[] id_list = id_str.Split(',');
            List<WaterOrder> order_list = new List<WaterOrder>();
            foreach (String it in id_list) {
                try
                {
                    long id = long.Parse(it);
                    var waterOrder = await _context.WaterOrder
               .Include(p => p.client)
               .Include(p => p.address)
               .Include(p => p.user)
               .Where(p => p.id == id).ToListAsync();

                    if (waterOrder == null || waterOrder.Count == 0)
                    {
                        continue;
                    }

                    waterOrder.First().items = await _context.WaterOrderItem
                     .Include(p => p.product)
                     .Where(p => p.WaterOrderid == waterOrder.First().id)
                     .ToListAsync();
                    order_list.Add(waterOrder.First());
                }
                catch (FormatException) { }
                

            }


            return order_list;
        }


        [HttpGet("getWaterOrderedProductList")]
        public async Task<ActionResult<IEnumerable<WaterOrderedProduct>>> getWaterOrderedProductList([FromQuery] DateTime begin_date,[FromQuery] DateTime end_date)
        {

            String beginDateStr = begin_date.ToString("yyyy-MM-dd hh:mm:ss");
            String endDateStr = end_date.ToString("yyyy-MM-dd hh:mm:ss");
            return await _context.WaterOrderedProduct.FromSqlRaw("" +
                "" +
                " SELECT "+
                " sum(woi.qty) as product_qty, "+
                " wp.name as product_name  "+
                " FROM water_order_item  woi  "+
                " LEFT JOIN water_product wp ON wp.id = woi.\"WaterProductid\" "+
                " WHERE woi.order_item_accepted_status = false  "+
                " AND woi.updated_date_time BETWEEN '"+ beginDateStr + "' AND '"+ endDateStr + "' "+
                " GROUP BY wp.name ").ToListAsync();
        }


        // GET: api/WaterOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterOrder>> GetWaterOrder(long id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }

            return waterOrder;
        }

        [HttpGet("getOrderFullInfoByIdForUpdate")]
        public async Task<ActionResult<WaterOrder>> getOrderFullInfoByIdForUpdate([FromQuery]long id)
        {
            var waterOrder = await _context.WaterOrder
                .Include(p => p.client)
                .Include(p => p.address)
                .Include(p => p.user)
                .Where(p => p.id == id).ToListAsync();

            if (waterOrder == null || waterOrder.Count == 0)
            {
                return NotFound();
            }

            waterOrder.First().items = await _context.WaterOrderItem
             .Include(p => p.product)
             .Where(p => p.WaterOrderid == waterOrder.First().id)
             .ToListAsync();

            return waterOrder.First(); ;
        }

        [HttpGet("getOrderFullInfoByid")]
        public async Task<ActionResult<WaterOrder>> getOrderFullInfoByid([FromQuery]long order_id)
        {
            var waterOrder = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.id == order_id).ToListAsync();

            if (waterOrder == null || waterOrder.Count == 0)
            {
                return NotFound();
            }

            waterOrder.First().items = await _context.WaterOrderItem
                .Include(p =>p.product)
                .Where(p =>p.WaterOrderid == waterOrder.First().id)
                .ToListAsync();


            return waterOrder.First();
        }

        [HttpGet("getLastOrderTwoOrdersFullInfoByid")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> getLastOrderFullInfoByid([FromQuery] long client_id)
        {
            var waterOrder = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Include(p => p.deleivered_user_auth)
                .ThenInclude(p => p.user)
                .Where(p => p.WaterClientid == client_id)
                .OrderByDescending( p => p.id).Take(2).ToListAsync();

            if (waterOrder == null || waterOrder.Count == 0)
            {
                return NotFound();
            }

            waterOrder.First().items = await _context.WaterOrderItem
                .Include(p => p.product)
                .Where(p => p.WaterOrderid == waterOrder.First().id)
                .ToListAsync();


            return waterOrder;
        }

        [HttpGet("closeAcceptOrder")]
        public async Task<ActionResult<WaterOrder>> closeAcceptOrder([FromQuery]long id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }
            if (waterOrder.accepted_status == true)
            {
                return NotFound("Already Accepted this order");
            }


            waterOrder.accepted_status = true;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        [HttpGet("addAcceptOrderToUsersWithAuthid")]
        public async Task<ActionResult<WaterOrder>> addAcceptOrderToUsersWithAuthid([FromQuery] long id, [FromQuery] long user_auth_id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }

            if (waterOrder.accepted_status == true) {
                return NotFound("Already Accepted this order");
            }

            waterOrder.accepted_status = true;
            waterOrder.deleivered_user_auth_id = user_auth_id;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        [HttpGet("addOrderToUsersWithAuthid")]
        public async Task<ActionResult<WaterOrder>> addOrderToUsersWithAuthid([FromQuery] long id,[FromQuery] long user_auth_id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }
            waterOrder.deleivered_user_auth_id = user_auth_id;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        [HttpGet("addDeOrderToUsersWithAuthidRemoveCar")]
        public async Task<ActionResult<WaterOrder>> addDeOrderToUsersWithAuthidRemoveCar([FromQuery] long id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }
            waterOrder.deleivered_user_auth_id = null;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }


        [HttpGet("closeAcceptOrderWithAllitemsTogather")]
        public async Task<ActionResult<WaterOrder>> closeAcceptOrderWithAllitemsTogather([FromQuery] long id,[FromQuery] double olingan_bakalshka_soni)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);

            if (waterOrder == null)
            {
                return NotFound();
            }
            if (waterOrder.accepted_status == true) {
                return NotFound("Already accepted order");
            }

            waterOrder.accepted_status = true;

            List<WaterOrderItem> items = await _context.WaterOrderItem
                .Include(p => p.product)
                .Where(p =>p.WaterOrderid == id && p.order_item_accepted_status == false)
                .ToListAsync();

            if (items != null && items.Count > 0) {

                foreach (WaterOrderItem it in items) {
                    it.order_item_accepted_status = true;

                    if (it.product.main_product == true) {

                        await addBotlfInfoToClientForAccept(olingan_bakalshka_soni, it.real_qty,
                            it.WaterProductid, waterOrder.WaterClientid, waterOrder.WaterClientAddressid);

                    }
                    
                }

                _context.WaterOrderItem.UpdateRange(items);
                await _context.SaveChangesAsync();
            }
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        [HttpGet("updateOrderFullReturningInfo")]
        public async Task<ActionResult<WaterOrder>> updateOrderFullReturningInfo(
            [FromQuery] long order_id,
            [FromQuery] double olingan_bakalshka_soni,
            [FromQuery] double berilgan_bakalshka_soni,
            [FromQuery] long water_client_bottle_info,
            [FromQuery] long check_id)
        {


            var waterOrder = await _context.WaterOrder.FindAsync(order_id);

            if (waterOrder == null)
            {
                return NotFound();
            }



            //update order
            waterOrder.reserverd_numeric_id_1 = olingan_bakalshka_soni;
            waterOrder.reserverd_numeric_id_2 = berilgan_bakalshka_soni;
            waterOrder.reserverd_number_id_1 = water_client_bottle_info;
            waterOrder.reserverd_number_id_2 = check_id;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }


        [HttpGet("deAgainAcceptAlredyPlannedOrderFullReturningInfo")]
        public async Task<ActionResult<WaterOrder>> deAgainAcceptAlredyPlannedOrderFullReturningInfo(
    [FromQuery] long order_id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(order_id);

            if (waterOrder == null)
            {
                return NotFound();
            }

            if (waterOrder.accepted_status == false) {
                return NotFound("Bajarilmagandi qaytara olmesz");
            }

            //bakalashkani orqaga qaytardim

            try
            {
                var waterbottleInfo = await _context.WaterClientBottleInfo.FindAsync(waterOrder.reserverd_number_id_1);
                if (waterbottleInfo != null)
                {
                    double berilgan_baklashka_soni = waterOrder.reserverd_numeric_id_2 ?? 0.0;
                    double olingan_bakalshka_soni = waterOrder.reserverd_numeric_id_1 ?? 0.0;

                    waterbottleInfo.bottle_count = waterbottleInfo.bottle_count + olingan_bakalshka_soni-berilgan_baklashka_soni;
                    waterbottleInfo.bottle_count_real = waterbottleInfo.bottle_count_real  + olingan_bakalshka_soni - berilgan_baklashka_soni;
                    _context.WaterClientBottleInfo.Update(waterbottleInfo);
                    await _context.SaveChangesAsync();
                }

               
            }
            catch (Exception) { }

            //check udalit qilindi
            try
            {
                var waterCheck = await _context.WaterCheck.FindAsync(waterOrder.reserverd_number_id_2);
                if (waterCheck != null)
                {
                    _context.WaterCheck.Remove(waterCheck);
                    await _context.SaveChangesAsync();
                }


            }
            catch (Exception) { }




            //update order
            waterOrder.accepted_status = false;
            waterOrder.reserverd_numeric_id_1 = null;
            waterOrder.reserverd_numeric_id_2 = null;
            waterOrder.reserverd_number_id_1 = null;
            waterOrder.reserverd_number_id_2 = null;
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }



        [HttpGet("addBotlfInfoToClientForAccept")]
    public async Task<ActionResult<WaterClientBottleInfo>> addBotlfInfoToClientForAccept(
    [FromQuery] double olingan_bakalshka_soni,
    [FromQuery] double berilgan_bakalshka_soni,
    [FromQuery] long product_id,
    [FromQuery] long client_id,
    [FromQuery] long address_id)
        {
            var waterClientBottleInfo = await _context.WaterClientBottleInfo.Where(p => p.WaterClientid == client_id
            && p.WaterClientAddressid == address_id && (p.WaterProductid == product_id)).ToListAsync();


            WaterMessageLog messageLog = new WaterMessageLog();
            messageLog.active_status = true;
            WaterClient client = await _context.WaterClient.FindAsync(client_id);
            WaterProduct product = await _context.WaterProduct.FindAsync(product_id);
            WaterClientAddress clientAddress = await _context.WaterClientAddress.FindAsync(address_id);

            String message_str =
                  " Klient: " + client.fio + " "
                + " Adress: " + clientAddress.address + "  "
                + " Tovar: " + product.name + " "
                + " Olingan soni: " + olingan_bakalshka_soni.ToString() + " "
                + " Berilgan soni: " + berilgan_bakalshka_soni.ToString();
            messageLog.message_str = message_str;

            _context.WaterMessageLog.Update(messageLog);
            await _context.SaveChangesAsync();



            if (waterClientBottleInfo == null || waterClientBottleInfo.Count() == 0)
            {
                //topilmadi yangi yaratamiz
                WaterClientBottleInfo info = new WaterClientBottleInfo();
                info.bottle_count = olingan_bakalshka_soni + berilgan_bakalshka_soni;
                info.bottle_count_real = berilgan_bakalshka_soni;
                info.WaterProductid = product_id;
                info.WaterClientAddressid = address_id;
                info.WaterClientid = client_id;
                info.active_status = true;
                _context.WaterClientBottleInfo.Update(info);
                await _context.SaveChangesAsync();
                return info;
            }
            else
            {
                waterClientBottleInfo.First().bottle_count = waterClientBottleInfo.First().bottle_count + (olingan_bakalshka_soni + berilgan_bakalshka_soni);
                waterClientBottleInfo.First().bottle_count_real = waterClientBottleInfo.First().bottle_count_real + berilgan_bakalshka_soni;
                _context.WaterClientBottleInfo.Update(waterClientBottleInfo.First());
                await _context.SaveChangesAsync();
            }

            return waterClientBottleInfo.First();
        }


        [HttpGet("getPagination")]
        public async Task<ActionResult<TexPaginationModel>> getPagination([FromQuery] int page, [FromQuery] int size)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }

            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpGet("getPaginationAllByDateTme")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationAllByDateTme([FromQuery] int page,
            [FromQuery] int size, [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true
                && (p.order_date >= begin_date && p.order_date <= end_date))
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && (p.order_date >= begin_date && p.order_date <= end_date)).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationAllAcceptedByDateTme")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationAllAcceptedByDateTme([FromQuery] int page,
    [FromQuery] int size, [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.accepted_status == true
                && (p.order_date >= begin_date && p.order_date <= end_date))
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.accepted_status == true
            && (p.order_date >= begin_date && p.order_date <= end_date)).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpGet("getPaginationAllNotAcceptedByDateTme")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationAllNotAcceptedByDateTme([FromQuery] int page,
[FromQuery] int size, [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.accepted_status == false
                && (p.order_date >= begin_date && p.order_date <= end_date))
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                   
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.accepted_status == false
            && (p.order_date >= begin_date && p.order_date <= end_date)).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationOpenOrdersList")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationOpenOrdersList([FromQuery] int page, [FromQuery] int size)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true && p.accepted_status == false)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder it in categoryList) {

                WaterOrder order = await _context.WaterOrder.FindAsync(it.id);
                if (order != null)
                {

                    List<WaterOrderItem> orderItems = await _context.WaterOrderItem
                        .Include(p => p.product)
                        .Where(p => p.WaterOrderid == order.id).ToListAsync();

                    if (orderItems != null)
                    {

                        String product_name_list = "";
                        String product_name_list1 = "";
                        String product_name_list2 = "";
                        foreach (WaterOrderItem itmord in orderItems)
                        {
                            product_name_list = product_name_list + itmord.product.name + "  ";
                            product_name_list1 = product_name_list1 + itmord.product.name + "_" + itmord.qty.ToString() + "  ";
                            product_name_list2 = product_name_list2 + itmord.product.name + "==" + itmord.qty.ToString() + "  ";
                        }

                        it.name_pp = product_name_list;
                        it.name_pp1 = product_name_list1;
                        it.name_pp2 = product_name_list2;


                    }

                }




                List<WaterClientPhoneNumber> phone_numbers = await _context
                    .WaterClientPhoneNumber.Where(p=>p.WaterClientid == it.WaterClientid).ToListAsync();

                double val_date = it.order_date.Date.Subtract(DateTime.Now.Date).TotalDays;
                it.days_count = val_date;
                if (val_date < 0) {
                    it.color_value = "black";
                }

                if (val_date == 0.0)
                {
                    it.color_value = "red";
                }

                if (val_date > 0.0)
                {
                    it.color_value = "green";
                }

                String phones_list_str_arr = "";

             if (phone_numbers != null && phone_numbers.Count > 0){

                    foreach (WaterClientPhoneNumber itm in phone_numbers){

                        phones_list_str_arr = phones_list_str_arr +" "+ itm.phone_number;

                    }

                }

                it.phone_number_list_arr = phones_list_str_arr;

            }

            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true && p.accepted_status == false).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationBeatweanDateWithoutTimeOpenOrdersListByAuthIdAsc")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationBeatweanDateWithoutTimeOpenOrdersListByAuthIdAsc([FromQuery] int page,
[FromQuery] int size,
[FromQuery] DateTime begin_date, [FromQuery] DateTime end_date, [FromQuery] long user_auth_id)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true
                && p.accepted_status == false
                && (p.order_date >= begin_date && p.order_date <= end_date)
                && p.deleivered_user_auth_id == user_auth_id)
                .Skip(page * size).Take(size).OrderBy(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }

            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && p.accepted_status == false
            && (p.order_date >= begin_date && p.order_date <= end_date)
            && p.deleivered_user_auth_id == user_auth_id).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationBeatweanDateWithoutTimeOpenOrdersListByAuthId")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationBeatweanDateWithoutTimeOpenOrdersListByAuthId([FromQuery] int page,
    [FromQuery] int size,
    [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date,[FromQuery] long user_auth_id)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true
                && p.accepted_status == false
                && (p.order_date >= begin_date && p.order_date <= end_date)
                && p.deleivered_user_auth_id == user_auth_id)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && p.accepted_status == false
            && (p.order_date >= begin_date && p.order_date <= end_date)
            && p.deleivered_user_auth_id == user_auth_id).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationBeatweanDateWithoutTimeOpenOrdersListByNotAddedAnyUser")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationBeatweanDateWithoutTimeOpenOrdersListByNotAddedAnyUser([FromQuery] int page,
    [FromQuery] int size,
    [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true
                && p.accepted_status == false
                && (p.order_date >= begin_date && p.order_date <= end_date)
                && p.deleivered_user_auth_id == null)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && p.accepted_status == false
            && (p.order_date >= begin_date && p.order_date <= end_date)
            && p.deleivered_user_auth_id == null).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }


        [HttpGet("getPaginationBeatweanDateWithoutTimeOpenOrdersList")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationBeatweanDateWithoutTimeOpenOrdersList([FromQuery] int page,
            [FromQuery] int size,
            [FromQuery] DateTime begin_date,[FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Where(p => p.active_status == true
                && p.accepted_status == false
                && (p.order_date >= begin_date && p.order_date<= end_date))
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }

            foreach (WaterOrder itm_o in categoryList) {
                itm_o.user = await _context.WaterUser.FindAsync(itm_o.WaterUserid);
                itm_o.client = await _context.WaterClient.FindAsync(itm_o.WaterClientid);
                itm_o.address = await _context.WaterClientAddress.FindAsync(itm_o.WaterClientAddressid);


                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null) {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr) {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list  + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }

            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && p.accepted_status == false
            && (p.order_date >= begin_date && p.order_date <= end_date)).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpGet("getPaginationBeatweanDateWithoutTimeClosedOrdersList")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationBeatweanDateWithoutTimeClosedOrdersList([FromQuery] int page,
    [FromQuery] int size,
    [FromQuery] DateTime begin_date, [FromQuery] DateTime end_date)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true
                && p.accepted_status == true
                && (p.order_date >= begin_date && p.order_date <= end_date))
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true
            && p.accepted_status == true
            && (p.order_date >= begin_date && p.order_date <= end_date)).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpGet("getPaginationClosedOrdersList")]
        public async Task<ActionResult<TexPaginationModel>> getPaginationClosedOrdersList([FromQuery] int page, [FromQuery] int size)
        {
            TexPaginationModel paginationModel = new TexPaginationModel();
            List<WaterOrder> categoryList = await _context.WaterOrder
                .Include(p => p.user)
                .Include(p => p.client)
                .Include(p => p.address)
                .Where(p => p.active_status == true && p.accepted_status == true)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterOrder>();
            }
            foreach (WaterOrder itm_o in categoryList)
            {

                List<WaterOrderItem> item_odr = await _context.WaterOrderItem
                 
                    .Where(p => p.WaterOrderid == itm_o.id)
                    .ToListAsync();

                if (item_odr == null)
                {
                    item_odr = new List<WaterOrderItem>();
                }

                String product_name_list = "";
                String product_name_list_with_qty = "";
                String product_name_list_with_qty_sign = "";

                foreach (WaterOrderItem itom in item_odr)
                {
                    itom.product = await _context.WaterProduct.FindAsync(itom.WaterProductid);
                    product_name_list = product_name_list + itom.product.name;
                    product_name_list_with_qty = product_name_list_with_qty + itom.product.name + " (" + itom.qty.ToString() + ")   ";
                    product_name_list_with_qty_sign = product_name_list_with_qty_sign + itom.product.name + "==" + itom.qty.ToString() + "  ";
                }

                itm_o.name_pp = product_name_list;
                itm_o.name_pp1 = product_name_list_with_qty;
                itm_o.name_pp2 = product_name_list_with_qty_sign;


            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterOrder.Where(p => p.active_status == true && p.accepted_status == true).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        // PUT: api/WaterOrders/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterOrder(long id, WaterOrder waterOrder)
        {
            if (id != waterOrder.id)
            {
                return BadRequest();
            }

            _context.Entry(waterOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WaterOrders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WaterOrder>> PostWaterOrder(WaterOrder waterOrder)
        {
            _context.WaterOrder.Update(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        [HttpPost("updateOrAddOrderList")]
        public async Task<ActionResult<IEnumerable<WaterOrder>>> updateOrAddOrderList(List<WaterOrder> waterOrderList)
        {
            _context.WaterOrder.UpdateRange(waterOrderList);
            await _context.SaveChangesAsync();

            return waterOrderList;
        }


        [HttpPost("updateOrAddOrderOnlyWithItems")]
        public async Task<ActionResult<WaterOrder>> updateOrAddOrderOnlyWithItems(WaterOrder order)
        {
            _context.WaterOrder.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        // GET: api/WaterOrders/getClientsWithoutOrdersAfterDate
        [HttpGet("getClientsWithoutOrdersAfterDate")]
        public async Task<ActionResult<IEnumerable<WaterClient>>> getClientsWithoutOrdersAfterDate([FromQuery] string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Date parameter is required");
            }

            DateTime targetDate;
            if (!DateTime.TryParse(date, out targetDate))
            {
                return BadRequest("Invalid date format");
            }

            // Barcha klientlarni olish
            var allClients = await _context.WaterClient
                .Include(c => c.addresses)
                    .ThenInclude(a => a.tuman)
                .Include(c => c.phone_numbers_list)
                .Include(c => c.tuman)
                .Where(c => c.active_status == true)
                .ToListAsync();

            // Tanlangan sanadan keyin har qanday zakaz bergan klientlar (accepted_status dan qat'iy nazar)
            var clientsWithOrders = await _context.WaterOrder
                .Where(o => o.order_date >= targetDate && o.accepted_status == false)
                .Select(o => o.WaterClientid)
                .Distinct()
                .ToListAsync();

            // Zakaz bermagan klientlar
            var clientsWithoutOrders = allClients
                .Where(c => !clientsWithOrders.Contains(c.id))
                .ToList();

            // Har bir klientning oxirgi zakazini tekshirish va otmen zakazlarni filtrlash
            var clientsToRemove = new List<WaterClient>();
            foreach (var client in clientsWithoutOrders)
            {
                // Klientning oxirgi zakazini topish (tanlangan sanadan oldin)
                var lastOrder = await _context.WaterOrder
                    .Where(o => o.WaterClientid == client.id && o.order_date < targetDate)
                    .OrderByDescending(o => o.order_date)
                    .FirstOrDefaultAsync();

                // Agar oxirgi zakaz otmen zakaz bo'lsa (reserverd_note_3 == "orange" va reserverd_note_2 == "5")
                if (lastOrder != null && 
                    lastOrder.reserverd_note_3 == "orange" && 
                    lastOrder.reserverd_note_2 == "5")
                {
                    clientsToRemove.Add(client);
                }
            }

            // Otmen zakazga ega bo'lgan klientlarni ro'yxatdan olib tashlash
            clientsWithoutOrders = clientsWithoutOrders
                .Where(c => !clientsToRemove.Contains(c))
                .ToList();

            // Asosiy product'ni topish
            var mainProduct = await _context.WaterProduct
                .Where(p => p.main_product == true)
                .FirstOrDefaultAsync();

            // Barcha active bottle info ma'lumotlarini olish
            var allBottleInfos = await _context.WaterClientBottleInfo
                .Include(bi => bi.product)
                .Where(bi => bi.active_status == true)
                .ToListAsync();

            // Har bir client va address uchun bottle count ni to'ldirish
            foreach (var client in clientsWithoutOrders)
            {
                if (client.addresses != null)
                {
                    foreach (var address in client.addresses)
                    {
                        // Avval asosiy product uchun bottle info ni topish
                        var bottleInfo = mainProduct != null 
                            ? allBottleInfos
                                .FirstOrDefault(bi => bi.WaterClientid == client.id && 
                                                     bi.WaterClientAddressid == address.id &&
                                                     bi.WaterProductid == mainProduct.id)
                            : null;
                        
                        // Agar asosiy product uchun topilmasa, barcha product'lar uchun birinchi topilganini olish
                        if (bottleInfo == null)
                        {
                            bottleInfo = allBottleInfos
                                .FirstOrDefault(bi => bi.WaterClientid == client.id && 
                                                     bi.WaterClientAddressid == address.id);
                        }
                        
                        if (bottleInfo != null)
                        {
                            address.bottle_count = bottleInfo.bottle_count;
                            address.bottle_count_real = bottleInfo.bottle_count_real;
                        }
                        else
                        {
                            address.bottle_count = 0;
                            address.bottle_count_real = 0;
                        }
                    }
                }
            }

            return clientsWithoutOrders;
        }

        // POST: api/WaterOrders/cancelMultipleClients
        [HttpPost("cancelMultipleClients")]
        public async Task<ActionResult> cancelMultipleClients([FromBody] CancelMultipleClientsDto dto)
        {
            if (dto == null || dto.ClientIds == null || dto.ClientIds.Count == 0)
            {
                return BadRequest("ClientIds are required");
            }

            var results = new List<object>();
            var mainProduct = await _context.WaterProduct
                .Where(p => p.main_product == true)
                .FirstOrDefaultAsync();

            if (mainProduct == null)
            {
                return BadRequest("Main product not found");
            }

            // Sana formatlash
            DateTime selectedDate;
            if (!DateTime.TryParse(dto.OrderDate, out selectedDate))
            {
                return BadRequest("Invalid date format");
            }
            string dateStr = selectedDate.ToString("dd.MM.yyyy");

            foreach (var clientId in dto.ClientIds)
            {
                try
                {
                    var client = await _context.WaterClient
                        .Include(c => c.addresses)
                        .FirstOrDefaultAsync(c => c.id == clientId);

                    if (client == null || client.addresses == null || client.addresses.Count == 0)
                    {
                        results.Add(new { clientId, success = false, message = "Client or address not found" });
                        continue;
                    }

                    // Ertangi kunni hisoblash
                    var tomorrow = DateTime.Today.AddDays(1);
                    var tomorrowDateStr = tomorrow.ToString("yyyy-MM-dd") + "T12:00:00.000Z";

                    // Har bir manzil uchun otmen zakaz yaratish
                    var orderIds = new List<long>();
                    foreach (var address in client.addresses)
                    {
                        // Otmen zakaz yaratish
                        var cancelOrder = new WaterOrder
                        {
                            order_date = DateTime.Parse(tomorrowDateStr),
                            WaterClientid = clientId,
                            water_count = 0,
                            WaterClientAddressid = address.id,
                            note = "Otmen -- " + dateStr + " sanadan beri suv olmagan",
                            reserverd_note_3 = "orange",
                            reserverd_note_2 = "5", // otmen bugnini bilish uchun
                            accepted_status = false,
                            items = new List<WaterOrderItem>()
                        };

                        _context.WaterOrder.Add(cancelOrder);
                        await _context.SaveChangesAsync();
                        orderIds.Add(cancelOrder.id);
                    }

                    results.Add(new { clientId, success = true, orderIds = orderIds, addressesCount = client.addresses.Count });
                }
                catch (Exception ex)
                {
                    results.Add(new { clientId, success = false, message = ex.Message });
                }
            }

            return Ok(new { results });
        }

        public class CancelMultipleClientsDto
        {
            public List<long> ClientIds { get; set; }
            public string OrderDate { get; set; }
            public string Note { get; set; }
        }

        // DELETE: api/WaterOrders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterOrder>> DeleteWaterOrder(long id)
        {
            var waterOrder = await _context.WaterOrder.FindAsync(id);
            if (waterOrder == null)
            {
                return NotFound();
            }

            _context.WaterOrder.Remove(waterOrder);
            await _context.SaveChangesAsync();

            return waterOrder;
        }

        // GET: api/WaterOrders/getPostavchikDailyStatistics
        [HttpGet("getPostavchikDailyStatistics")]
        public async Task<ActionResult> getPostavchikDailyStatistics(
            [FromQuery] DateTime begin_date,
            [FromQuery] DateTime end_date,
            [FromQuery] long auth_id)
        {
            try
            {
                // Bajarilgan zakazlarni olish
                var orders = await _context.WaterOrder
                    .Where(o => o.deleivered_user_auth_id == auth_id
                        && o.accepted_status == true
                        && o.order_accepted_date >= begin_date 
                        && o.order_accepted_date <= end_date)
                    .ToListAsync();

                // To'lov ma'lumotlarini olish (WaterCheck dan) - optimallashtirilgan
                var checkIds = orders
                    .Where(o => o.reserverd_number_id_2 != null && o.reserverd_number_id_2 > 0)
                    .Select(o => o.reserverd_number_id_2.Value)
                    .Distinct()
                    .ToList();

                var checks = await _context.WaterCheck
                    .Where(c => checkIds.Contains(c.id))
                    .ToListAsync();

                var checkDict = checks.ToDictionary(c => c.id);

                double totalCash = 0.0;
                double totalCard = 0.0;
                
                foreach (var order in orders)
                {
                    if (order.reserverd_number_id_2 != null && checkDict.ContainsKey(order.reserverd_number_id_2.Value))
                    {
                        var check = checkDict[order.reserverd_number_id_2.Value];
                        totalCash += check.cash;
                        totalCard += check.card;
                    }
                }

                // Kunlik statistika - order_accepted_date dan foydalanish
                var dailyStats = orders
                    .GroupBy(o => o.order_accepted_date.Date)
                    .Select(g => new
                    {
                        date = g.Key.ToString("yyyy-MM-dd"),
                        tarqatilgan_suv = g.Sum(o => o.reserverd_numeric_id_2 ?? 0.0),
                        olingan_idish = g.Sum(o => Math.Abs(o.reserverd_numeric_id_1 ?? 0.0))
                    })
                    .OrderBy(s => s.date)
                    .ToList();

                // Jami statistika
                var totalStats = new
                {
                    jami_tarqatilgan_suv = dailyStats.Sum(s => s.tarqatilgan_suv),
                    jami_olingan_idish = dailyStats.Sum(s => s.olingan_idish),
                    ishlagan_kunlar = dailyStats.Count,
                    jami_naqd = totalCash,
                    jami_plastik = totalCard,
                    jami_summa = totalCash + totalCard
                };

                return Ok(new
                {
                    daily_stats = dailyStats,
                    total_stats = totalStats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/WaterOrders/getRealTimePostavchikStatistics
        [HttpGet("getRealTimePostavchikStatistics")]
        public async Task<ActionResult> getRealTimePostavchikStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                // Bugungi kundagi barcha zakazlarni olish
                var todayOrders = await _context.WaterOrder
                    .Include(o => o.deleivered_user_auth)
                        .ThenInclude(a => a.user)
                    .Where(o => o.active_status == true
                        && ((o.accepted_status == true && o.order_accepted_date >= today && o.order_accepted_date < tomorrow)
                            || (o.accepted_status == false && o.order_date >= today && o.order_date < tomorrow)))
                    .ToListAsync();

                // Dostavchiklar bo'yicha guruhlash
                var postavchikStats = todayOrders
                    .Where(o => o.deleivered_user_auth_id != null)
                    .GroupBy(o => new
                    {
                        auth_id = o.deleivered_user_auth_id.Value,
                        user_fio = o.deleivered_user_auth != null && o.deleivered_user_auth.user != null 
                            ? o.deleivered_user_auth.user.fio 
                            : "Noma'lum"
                    })
                    .Select(g => new
                    {
                        auth_id = g.Key.auth_id,
                        user_fio = g.Key.user_fio,
                        tarqatilgan_suv = g.Where(o => o.accepted_status == true 
                            && o.order_accepted_date >= today 
                            && o.order_accepted_date < tomorrow)
                            .Sum(o => o.reserverd_numeric_id_2 ?? 0.0),
                        tarqatilishi_kerak = g.Where(o => o.accepted_status == false 
                            && o.order_date >= today 
                            && o.order_date < tomorrow)
                            .Sum(o => (double)o.water_count),
                        olingan_baklashka = g.Where(o => o.accepted_status == true 
                            && o.order_accepted_date >= today 
                            && o.order_accepted_date < tomorrow)
                            .Sum(o => Math.Abs(o.reserverd_numeric_id_1 ?? 0.0))
                    })
                    .OrderByDescending(s => s.tarqatilgan_suv)
                    .ToList();

                return Ok(postavchikStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/WaterOrders/getDashboardStatistics
        [HttpGet("getDashboardStatistics")]
        public async Task<ActionResult> getDashboardStatistics(
            [FromQuery] DateTime? begin_date,
            [FromQuery] DateTime? end_date,
            [FromQuery] DateTime? compare_begin_date,
            [FromQuery] DateTime? compare_end_date)
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                // 1. Bugungi kun statistikasi
                var todayOrders = await _context.WaterOrder
                    .Where(o => o.active_status == true
                        && ((o.accepted_status == true && o.order_accepted_date >= today && o.order_accepted_date < tomorrow)
                            || (o.accepted_status == false && o.order_date >= today && o.order_date < tomorrow)))
                    .ToListAsync();

                var todayCheckIds = todayOrders
                    .Where(o => o.reserverd_number_id_2 != null && o.reserverd_number_id_2 > 0)
                    .Select(o => o.reserverd_number_id_2.Value)
                    .Distinct()
                    .ToList();

                var todayChecks = await _context.WaterCheck
                    .Where(c => todayCheckIds.Contains(c.id))
                    .ToListAsync();

                var todayCheckDict = todayChecks.ToDictionary(c => c.id);

                double todayCash = 0.0;
                double todayCard = 0.0;

                foreach (var order in todayOrders)
                {
                    if (order.reserverd_number_id_2 != null && todayCheckDict.ContainsKey(order.reserverd_number_id_2.Value))
                    {
                        var check = todayCheckDict[order.reserverd_number_id_2.Value];
                        todayCash += check.cash;
                        todayCard += check.card;
                    }
                }

                var todayStats = new
                {
                    tarqatilgan_suv = todayOrders.Where(o => o.accepted_status == true 
                        && o.order_accepted_date >= today && o.order_accepted_date < tomorrow)
                        .Sum(o => o.reserverd_numeric_id_2 ?? 0.0),
                    tarqatilishi_kerak = todayOrders.Where(o => o.accepted_status == false 
                        && o.order_date >= today && o.order_date < tomorrow)
                        .Sum(o => (double)o.water_count),
                    yigilgan_baklashka = todayOrders.Where(o => o.accepted_status == true 
                        && o.order_accepted_date >= today && o.order_accepted_date < tomorrow)
                        .Sum(o => Math.Abs(o.reserverd_numeric_id_1 ?? 0.0)),
                    topilgan_pul = todayCash + todayCard
                };

                // 2. Sana oralig'i statistikasi
                object periodStats;

                if (begin_date.HasValue && end_date.HasValue)
                {
                    var periodOrders = await _context.WaterOrder
                        .Where(o => o.active_status == true
                            && o.accepted_status == true
                            && o.order_accepted_date >= begin_date.Value
                            && o.order_accepted_date <= end_date.Value.AddDays(1).AddTicks(-1))
                        .ToListAsync();

                    // To'lov ma'lumotlarini olish
                    var periodCheckIds = periodOrders
                        .Where(o => o.reserverd_number_id_2 != null && o.reserverd_number_id_2 > 0)
                        .Select(o => o.reserverd_number_id_2.Value)
                        .Distinct()
                        .ToList();

                    var periodChecks = await _context.WaterCheck
                        .Where(c => periodCheckIds.Contains(c.id))
                        .ToListAsync();

                    var periodCheckDict = periodChecks.ToDictionary(c => c.id);

                    double periodTotalCash = 0.0;
                    double periodTotalCard = 0.0;

                    foreach (var order in periodOrders)
                    {
                        if (order.reserverd_number_id_2 != null && periodCheckDict.ContainsKey(order.reserverd_number_id_2.Value))
                        {
                            var check = periodCheckDict[order.reserverd_number_id_2.Value];
                            periodTotalCash += check.cash;
                            periodTotalCard += check.card;
                        }
                    }

                    // Kunlik to'lov ma'lumotlari
                    var dailyPayments = periodOrders
                        .Where(o => o.reserverd_number_id_2 != null && periodCheckDict.ContainsKey(o.reserverd_number_id_2.Value))
                        .GroupBy(o => o.order_accepted_date.Date)
                        .Select(g => new
                        {
                            date = g.Key.ToString("yyyy-MM-dd"),
                            daily_sum = g.Sum(o =>
                            {
                                if (o.reserverd_number_id_2 != null && periodCheckDict.ContainsKey(o.reserverd_number_id_2.Value))
                                {
                                    var check = periodCheckDict[o.reserverd_number_id_2.Value];
                                    return check.cash + check.card;
                                }
                                return 0.0;
                            })
                        })
                        .OrderBy(d => d.date)
                        .ToList();

                    periodStats = new
                    {
                        tarqatilgan_suv_soni = periodOrders.Sum(o => o.reserverd_numeric_id_2 ?? 0.0),
                        tarqatilgan_suv_summasi = periodTotalCash + periodTotalCard,
                        yangi_klientlar_soni = await _context.WaterClient
                            .Where(c => c.active_status == true
                                && c.created_date_time.HasValue
                                && c.created_date_time.Value >= begin_date.Value
                                && c.created_date_time.Value <= end_date.Value.AddDays(1).AddTicks(-1))
                            .CountAsync(),
                        otmen_klientlar_soni = await _context.WaterOrder
                            .Where(o => o.active_status == true
                                && o.order_date >= begin_date.Value
                                && o.order_date <= end_date.Value.AddDays(1).AddTicks(-1)
                                && (o.reserverd_note_2 == "5" || (o.note != null && o.note.Contains("Otmen"))))
                            .Select(o => o.WaterClientid)
                            .Distinct()
                            .CountAsync(),
                        daily_payments = dailyPayments
                    };
                }
                else
                {
                    periodStats = new
                    {
                        tarqatilgan_suv_soni = 0.0,
                        tarqatilgan_suv_summasi = 0.0,
                        yangi_klientlar_soni = 0,
                        otmen_klientlar_soni = 0,
                        daily_payments = new List<object>()
                    };
                }

                // 3. O'tgan yil bilan solishtirish
                var compareStats = new
                {
                    tarqatilgan_suv_soni = 0.0,
                    tarqatilgan_suv_summasi = 0.0,
                    yangi_klientlar_soni = 0,
                    otmen_klientlar_soni = 0
                };

                if (compare_begin_date.HasValue && compare_end_date.HasValue)
                {
                    var compareOrders = await _context.WaterOrder
                        .Where(o => o.active_status == true
                            && o.accepted_status == true
                            && o.order_accepted_date >= compare_begin_date.Value
                            && o.order_accepted_date <= compare_end_date.Value.AddDays(1).AddTicks(-1))
                        .ToListAsync();

                    // To'lov ma'lumotlarini olish
                    var compareCheckIds = compareOrders
                        .Where(o => o.reserverd_number_id_2 != null && o.reserverd_number_id_2 > 0)
                        .Select(o => o.reserverd_number_id_2.Value)
                        .Distinct()
                        .ToList();

                    var compareChecks = await _context.WaterCheck
                        .Where(c => compareCheckIds.Contains(c.id))
                        .ToListAsync();

                    var compareCheckDict = compareChecks.ToDictionary(c => c.id);

                    double compareTotalCash = 0.0;
                    double compareTotalCard = 0.0;

                    foreach (var order in compareOrders)
                    {
                        if (order.reserverd_number_id_2 != null && compareCheckDict.ContainsKey(order.reserverd_number_id_2.Value))
                        {
                            var check = compareCheckDict[order.reserverd_number_id_2.Value];
                            compareTotalCash += check.cash;
                            compareTotalCard += check.card;
                        }
                    }

                    compareStats = new
                    {
                        tarqatilgan_suv_soni = compareOrders.Sum(o => o.reserverd_numeric_id_2 ?? 0.0),
                        tarqatilgan_suv_summasi = compareTotalCash + compareTotalCard,
                        yangi_klientlar_soni = await _context.WaterClient
                            .Where(c => c.active_status == true
                                && c.created_date_time.HasValue
                                && c.created_date_time.Value >= compare_begin_date.Value
                                && c.created_date_time.Value <= compare_end_date.Value.AddDays(1).AddTicks(-1))
                            .CountAsync(),
                        otmen_klientlar_soni = await _context.WaterOrder
                            .Where(o => o.active_status == true
                                && o.order_date >= compare_begin_date.Value
                                && o.order_date <= compare_end_date.Value.AddDays(1).AddTicks(-1)
                                && (o.reserverd_note_2 == "5" || (o.note != null && o.note.Contains("Otmen"))))
                            .Select(o => o.WaterClientid)
                            .Distinct()
                            .CountAsync()
                    };
                }

                // 4. Foiz farqlarni hisoblash
                // Dynamic property access uchun reflection ishlatish
                double periodTarqatilganSuv = 0.0;
                int periodYangiKlientlar = 0;
                int periodOtmenKlientlar = 0;

                if (periodStats != null)
                {
                    var periodStatsType = periodStats.GetType();
                    var tarqatilganSuvProp = periodStatsType.GetProperty("tarqatilgan_suv_soni");
                    var yangiKlientlarProp = periodStatsType.GetProperty("yangi_klientlar_soni");
                    var otmenKlientlarProp = periodStatsType.GetProperty("otmen_klientlar_soni");

                    if (tarqatilganSuvProp != null)
                        periodTarqatilganSuv = (double)(tarqatilganSuvProp.GetValue(periodStats) ?? 0.0);
                    if (yangiKlientlarProp != null)
                        periodYangiKlientlar = (int)(yangiKlientlarProp.GetValue(periodStats) ?? 0);
                    if (otmenKlientlarProp != null)
                        periodOtmenKlientlar = (int)(otmenKlientlarProp.GetValue(periodStats) ?? 0);
                }

                var differences = new
                {
                    tarqatilgan_suv_foiz = periodTarqatilganSuv > 0 && compareStats.tarqatilgan_suv_soni > 0
                        ? ((periodTarqatilganSuv - compareStats.tarqatilgan_suv_soni) / compareStats.tarqatilgan_suv_soni) * 100
                        : 0.0,
                    tarqatilgan_suv_oq = periodTarqatilganSuv - compareStats.tarqatilgan_suv_soni,
                    yangi_klientlar_foiz = periodYangiKlientlar > 0 && compareStats.yangi_klientlar_soni > 0
                        ? ((double)(periodYangiKlientlar - compareStats.yangi_klientlar_soni) / compareStats.yangi_klientlar_soni) * 100
                        : 0.0,
                    yangi_klientlar_oq = periodYangiKlientlar - compareStats.yangi_klientlar_soni,
                    otmen_klientlar_foiz = periodOtmenKlientlar > 0 && compareStats.otmen_klientlar_soni > 0
                        ? ((double)(periodOtmenKlientlar - compareStats.otmen_klientlar_soni) / compareStats.otmen_klientlar_soni) * 100
                        : 0.0,
                    otmen_klientlar_oq = periodOtmenKlientlar - compareStats.otmen_klientlar_soni
                };

                var response = new
                {
                    today_stats = todayStats,
                    period_stats = periodStats,
                    compare_stats = compareStats,
                    differences = differences
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool WaterOrderExists(long id)
        {
            return _context.WaterOrder.Any(e => e.id == id);
        }
    }
}
