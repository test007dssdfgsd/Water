import Vue from 'vue'
import VueRouter from 'vue-router'


Vue.use(VueRouter)

const routes = [{
        path: '/',
        name: 'login',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/login/login.vue')
    },
    {
        path: '/province',
        name: 'province',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/province/province.vue')
    },
    {
        path: '/waiter',
        name: 'waiter',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/waiter/waiter.vue')  
    },
    {
        path: '/checkGroup/:id',
        name: 'checkGroup',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/check/checkGroup.vue')  
    },
    {
        path: '/testResultGroup/:id',
        name: 'testResultGroup',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/check/testResultGroup.vue')  
    },
    {
        path: '/payGroup/:id',
        name: 'payGroup',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/pay/payGroup.vue')  
    },
    {
        path: '/notPayed',
        name: 'notPayed',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/pay/notPayed.vue')  
    },
    // {
    //     path: '/check_date',
    //     name: 'check_date',
    //     meta: { layout: 'main' },
    //     component: () =>
    //         import ('../views/report/check_date.vue')  
    // },
    {
        path: '/rasxod_report',
        name: 'rasxod_report',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/rasxod_report.vue')  
    },
    {
        path: '/kassa',
        name: 'kassa',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/pay/kassa.vue')  
    },

    {
        path: '/debit_list',
        name: 'debit_list',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/debit/debit_list.vue')  
    },
    {
        path: '/payed_list',
        name: 'payed_list',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/debit/payed_list.vue')  
    },
    {
        path: '/province_add/:id',
        name: 'province_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/province/province_Add.vue')
    },
    {
        path: '/position',
        name: 'position',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/position/position.vue')
    },
    {
        path: '/position_add/:id',
        name: 'position_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/position/position_Add.vue')
    },
    {
        path: '/product',
        name: 'product',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/product/product.vue')
    },
    {
        path: '/product_add/:id',
        name: 'product_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/product/product_Add.vue')
    },
    {
        path: '/invoice',
        name: 'invoice',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/buy/buy.vue')
    },
    {
        path: '/invoice_add/:id',
        name: 'invoice_add',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/buy/buy_Add.vue')
    },
    {
        path: '/contragent',
        name: 'contragent',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/contragent/contragent.vue')
    },
    {
        path: '/contragent_add/:id',
        name: 'contragent_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/contragent/contragent_Add.vue')
    },
    {
        path: '/contragent_type',
        name: 'contragent_type',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/contragent_type/contragent_type.vue')
    },
    {
        path: '/contragent_type_add/:id',
        name: 'contragent_type_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/contragent_type/contragent_type_Add.vue')
    },
    {
        path: '/test',
        name: 'test',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/test/test.vue')
    },
    {
        path: '/test_add/:id',
        name: 'test_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/test/test_Add.vue')
    },
   
    
    {
        path: '/user',
        name: 'user',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/user/user.vue')
    },
    {
        path: '/user_add/:id',
        name: 'user_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/user/user_Add.vue')
    },
    {
        path: '/district',
        name: 'district',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/district/district.vue')
    },
    {
        path: '/district_add/:id',
        name: 'district_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/district/district_Add.vue')
    },
    {
        path: '/client',
        name: 'client',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/client/client.vue')
    },
    {
        path: '/client_add/:id',
        name: 'client_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/client/client_Add.vue')
    },
    {
        path: '/authorization/:id',
        name: 'authorization',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/user/authorization.vue')
    },
    
    {
        path: '/order',
        name: 'order',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/order/order.vue')
    },
    {
        path: '/order_add/:id',
        name: 'order_add',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/order/order_Add.vue')
    },
    {
        path: '/map_order',
        name: 'map_order',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/order/map_order.vue')
    },
   
    {
        path: '/know_mark_in_map',
        name: 'know_mark_in_map',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/order/know_mark_in_map.vue')
    },
    {
        path: '/new_order_list',
        name: 'new_order_list',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/order/new_order_list.vue')
    },
    {
        path: '/map_order_car',
        name: 'map_order_car',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/order_car/map_order_car.vue')
    },
    {
        path: '/pos_order_list',
        name: 'pos_order_list',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/order_car/pos_order_list.vue')
    },
    {
        path: '/map_order_postavchik',
        name: 'map_order_postavchik',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/order_car/map_order_postavchik.vue')
    },

    {
        path: '/postavchik_list',
        name: 'postavchik_list',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/postavchik/postavchik_list.vue')
    },
    {
        path: '/postavchik_map',
        name: 'postavchik_map',
        meta: { layout: 'empty' },
        component: () =>
            import ('../views/postavchik/postavchik_map.vue')
    },

    
    {
        path: '/check',
        name: 'check',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/check_date.vue')
    },
    {
        path: '/accept_order',
        name: 'accept_order',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/accept_order.vue')
    },
    {
        path: '/postov_money_report',
        name: 'postov_money_report',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/postov_money_report.vue')
    },
    {
        path: '/statistic',
        name: 'statistic',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/statistic.vue')
    },
    {
        path: '/deleteReport',
        name: 'deleteReport',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/deleteReport.vue')
    },
    {
        path: '/lastOrderReport',
        name: 'lastOrderReport',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/lastOrderReport.vue')
    },
    {
        path: '/client_cancel_report',
        name: 'client_cancel_report',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/client_cancel_report.vue')
    },
    {
        path: '/postavchik_statistics',
        name: 'postavchik_statistics',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/postavchik_statistics.vue')
    },
    {
        path: '/real_time_postavchik_stats',
        name: 'real_time_postavchik_stats',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/real_time_postavchik_stats.vue')
    },
    {
        path: '/dashboard',
        name: 'dashboard',
        meta: { layout: 'main' },
        component: () =>
            import ('../views/report/dashboard.vue')
    },
    


    // <----    Tegirmon  ---->

   

]

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
})


router.beforeEach((to, from, next) => {
    console.log(from)
if (to.path != '/') {
        if (localStorage.Login != '') {
            if (localStorage.Type == 1) {
                if (to.path === '/postavchik_list' || to.path === '/postavchik_map') {
                next()  // shu ikkitasiga ruxsat beramiz
                } else {
                next('/postavchik_list') // boshqalari qaytadi
                }
            } else if (localStorage.Type == 0) {
                next()
            } 
             else {
                next()
            }
            // else if (localStorage.Type == 2) {
            //     if (to.path != '/bemor' && to.path != '/doctor') {
            //         next('/bemor')
            //     }
            // }

            next()

        } else {
            next('/')
        }
    } else {
        next()
    }
})

export default router