import Vue from 'vue';
import Router from 'vue-router';
const _import = require('./_import_' + process.env.NODE_ENV);
import Full from '@/containers/Full';
Vue.use(Router);
export const constantRouterMap = [{
        path: '/login',
        component: _import('login/index'),
        hidden: true
    },
    {
        path: '/pages',
        redirect: '/pages/p404',
        name: 'Pages',
        component: {
            render(c) {
                return c('router-view');
            }
            // Full,
        },
        children: [{
                path: '404',
                name: 'Page404',
                component: _import('errorPages/Page404')
            },
            {
                path: '500',
                name: 'Page500',
                component: _import('errorPages/Page500')
            }
        ]
    }
];

export const asyncRouterMap = [{
        path: '/',
        redirect: '/dashboard',
        name: '首页',
        component: Full,
        children: [{
                path: '/dashboard',
                name: '介绍',
                icon: 'speedometer',
                component: r => require(['views/Dashboard'], r)
            },
            {
                path: '',
                name: '权限管理',
                icon: 'person-stalker',
                component: {
                    render(c) {
                        return c('router-view');
                    }
                },
                children: [{
                        path: '/roles',
                        name: '角色管理',
                        icon: 'person',
                        component: r => require(['views/manager/Role'], r)
                    },
                    {
                        path: '/users',
                        name: '用户管理',
                        icon: 'person-add',
                        component: r => require(['views/manager/Account'], r)
                    }
                ]
            },
            {
                path: '',
                name: '数据统计',
                icon: 'person-stalker',
                component: {
                    render(c) {
                        return c('router-view');
                    }
                },
                children: [{
                        path: '/sign',
                        name: '签到统计',
                        icon: 'person',
                        component: r => require(['views/statistical/sign'], r)
                    },
                    {
                        path: '/signdetail',
                        name: '签到明细',
                        icon: 'person-add',
                        component: r => require(['views/statistical/signdetail'], r)
                    }, {
                        path: '/warndevice',
                        name: '故障统计-设备',
                        icon: 'person-add',
                        component: r => require(['views/statistical/warndevice'], r)
                    }, {
                        path: '/warn',
                        name: '报警信息',
                        icon: 'person-add',
                        component: r => require(['views/statistical/warn'], r)
                    }, {
                        path: '/order',
                        name: '成交订单',
                        icon: 'person-add',
                        component: r => require(['views/statistical/order'], r)
                    }, {
                        path: '/productsale',
                        name: '产品销量',
                        icon: 'person-add',
                        component: r => require(['views/statistical/productsale'], r)
                    }, {
                        path: '/devicesale',
                        name: '设备销量',
                        icon: 'person-add',
                        component: r => require(['views/statistical/devicesale'], r)
                    }, {
                        path: '/areasale',
                        name: '区域销量',
                        icon: 'person-add',
                        component: r => require(['views/statistical/areasale'], r)
                    }, {
                        path: '/paytype',
                        name: '支付渠道',
                        icon: 'person-add',
                        component: r => require(['views/statistical/paytype'], r)
                    }, {
                        path: '/timearea',
                        name: '时段销量',
                        icon: 'person-add',
                        component: r => require(['views/statistical/timearea'], r)
                    }, {
                        path: '/store',
                        name: '订单信息',
                        icon: 'person-add',
                        component: r => require(['views/statistical/store'], r)
                    }, {
                        path: '/charge',
                        name: '充值记录',
                        icon: 'person-add',
                        component: r => require(['views/statistical/charge'], r)
                    }, {
                        path: '/activity',
                        name: '卡券记录',
                        icon: 'person-add',
                        component: r => require(['views/statistical/activity'], r)
                    }
                ]
            }
        ]
    },
    {
        path: '*',
        redirect: '/pages/404',
        hidden: true
    }
];

const temp = constantRouterMap.concat(asyncRouterMap);
export default new Router({
    mode: 'hash',
    linkActiveClass: 'open active',
    scrollBehavior: () => ({
        y: 0
    }),
    routes: temp
});