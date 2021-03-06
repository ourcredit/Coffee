import Vue from "vue"
import Router from "vue-router"

Vue.use(Router)

export default new Router({
  routes: [{
      path: "/author",
      name: "author",
      component: r => require(["@/components/Author"], r)
    },
    {
      path: "/",
      name: "container",
      component: r => require(["@/components/Container"], r),
      children: [{
          path: "/balance",
          name: "balance",
          component: r => require(["@/components/Balance"], r)
        },
        {
          path: "/detail/:id",
          name: "detail",
          component: r => require(["@/components/Detail"], r)
        },
        {
          path: "/mycard",
          name: "mycard",
          component: r => require(["@/components/MyCard"], r)
        },
        {
          path: "/order",
          name: "order",
          component: r => require(["@/components/Order"], r)
        },
        {
          path: "/activity",
          name: "activity",
          component: r => require(["@/components/Activity"], r)
        },
        {
          path: "/my",
          name: "my",
          component: r => require(["@/components/My"], r)
        }
      ]
    }
  ]
})
