import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { ACCESS_TOKEN_KEY } from '@/constants/storage'
import Cookies from 'js-cookie'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      component: () => import('@/layouts/GuestLayout.vue'),
      path: '/',
      name: 'home',
      children: [
        {
          path: '',
          name: 'guest-home',
          component: () => import('../views/HomeView.vue')
        },
        {
          path: 'auth',
          name: 'login-screen',
          component: () => import('../views/Auth/LoginScreen.vue')
        },
        {
          path: 'search',
          name: 'guest-search',
          component: () => import('../views/Guest/GuestSearch.vue')
        },
        {
          path: 'post/:id',
          name: 'post-detail-public',
          component: () => import('../views/Guest/PostBlog.vue')
        },
        {
          path: 'field/:id',
          name: 'public-field',
          component: () => import('../views/Guest/PublicField.vue')
        }
      ]
    },
    {
      path: '/user',
      component: () => import('@/layouts/UserLayout.vue'),
      beforeEnter: (to, from) => {
        const authStore = useAuthStore()
        if (authStore.identity && authStore.identity.roleName == 'Player') {
          return true
        }

        return { name: 'home' }
      },
      children: [
        {
          path: 'home',
          name: 'user-home',
          component: () => import('../views/User/ClientHome.vue')
        },
        {
          path: 'forget-password',
          name: 'user-forget-password',
          component: () => import('../views/User/ForgetPassword.vue')
        },
        {
          path: 'field/:id',
          name: 'user-field',
          component: () => import('../views/User/FieldDetail.vue')
        },
        {
          path: 'info',
          name: 'user-info',
          component: () => import('../views/User/ProfileUser.vue')
        },
        {
          path: 'free-search',
          name: 'free-search',
          component: () => import('../views/User/FreeSearch.vue')
        },
        {
          path: 'success-payment',
          name: 'success-payment',
          component: () => import('../views/User/SuccessPayment.vue')
        },
        {
          path: 'booking-history',
          name: 'booking-history',
          component: () => import('../views/User/BookingHistory.vue')
        },
        {
          path: 'post/:id',
          name: 'user-post-detail',
          component: () => import('../views/User/BlogPost.vue')
        },
        {
          path: 'cancel-payment',
          name: 'cancel-payment',
          component: () => import('../views/User/CancelPayment.vue')
        },
        {
          path: 'find-opponent',
          name: 'find-opponent',
          component: () => import('../views/User/FindOpponent.vue')
        },
        {
          path: 'find-opponent/mine/:page',
          name: 'find-opponent-mine',
          component: () => import('../views/User/FindOpponent.vue')
        },
        {
          path: 'find-opponent/:id',
          name: 'find-opponent-post',
          component: () => import('../views/User/OpponentFindingPost.vue')
        },
        {
          path: 'payment-history',
          name: 'paymment-history',
          component: () => import('../views/User/PaymentHistory.vue')
        },
        {
          path: 'favorite-fields',
          name: 'favorite-fields',
          component: () => import('../views/User/FavoriteFielde.vue')
        },
        {
          path: 'map-search',
          name: 'map-search',
          component: () => import('../views/User/MapSearch.vue')
        }
      ]
    },
    {
      path: '/field',
      component: () => import('@/layouts/FieldOwnerLayout.vue'),
      beforeEnter: (to, from) => {
        const authStore = useAuthStore()
        if (authStore.identity && authStore.identity.roleName == 'Field') {
          return true
        }
        return { name: 'home' }
      },
      children: [
        {
          path: 'home',
          name: 'owner-home',
          component: () => import('../views/FieldOwner/OwnerHome.vue')
        },
        {
          path: 'tournaments',
          name: 'tournaments',
          component: () => import('../views/FieldOwner/Tournaments.vue')
        },
        {
          path: 'list',
          name: 'list',
          component: () => import('../views/FieldOwner/MyFields.vue')
        },
        {
          path: 'bookings',
          name: 'bookings',
          component: () => import('../views/FieldOwner/Bookings.vue')
        },
        {
          path: 'schedules',
          name: 'schedules',
          component: () => import('../views/FieldOwner/ScheduleField.vue')
        },
        {
          path: 'statistics',
          name: 'statistics',
          component: () => import('../views/FieldOwner/StatisticView.vue')
        },
        {
          path: 'info',
          name: 'info',
          component: () => import('../views/FieldOwner/Info.vue')
        },
        {
          path: 'create-field',
          name: 'create-field-by-owner',
          component: () => import('../views/FieldOwner/CreateField.vue')
        },
        {
          path: 'update-field/:id',
          name: 'update-field-by-owner',
          component: () => import('../views/FieldOwner/UpdateField.vue')
        },
        {
          path: 'find-booking',
          name: 'find-booking',
          component: () => import('../views/FieldOwner/FindBooking.vue')
        },
        {
          path: 'field-detail/:id',
          name: 'field-detail',
          component: () => import('../views/FieldOwner/FieldDetail.vue')
        },
        {
          path: 'my-statistic',
          name: 'my-statistic',
          component: () => import('../views/FieldOwner/StatisticView.vue')
        },
        {
          path: 'staffs',
          name: 'staffs',
          component: () => import('../views/FieldOwner/StaffList.vue')
        },
        {
          path: 'posts',
          name: 'posts',
          component: () => import('../views/FieldOwner/PostList.vue')
        },
        {
          path: 'post-create',
          name: 'post-create',
          component: () => import('../views/FieldOwner/PostCreating.vue')
        },
        {
          path: 'post/:id',
          name: 'post-detail',
          component: () => import('../views/FieldOwner/PostBlogDetail.vue')
        },
        {
          path: 'payment-management',
          name: 'payment-management',
          component: () => import('../views/FieldOwner/PaymentManagement.vue')
        }
      ]
    },
    {
      path: '/staff',
      component: () => import('@/layouts/StaffLayout.vue'),
      beforeEnter: (to, from) => {
        const authStore = useAuthStore()
        if (authStore.identity && authStore.identity.roleName == 'Staff') {
          return true
        }
        return { name: 'home' }
      },
      children: [
        {
          path: 'home',
          name: 'staff-home',
          component: () => import('../views/FieldOwner/OwnerHome.vue')
        },
        {
          path: 'bookings',
          name: 'staff-bookings',
          component: () => import('../views/FieldOwner/Bookings.vue')
        },
        {
          path: 'schedules',
          name: 'staff-schedules',
          component: () => import('../views/FieldOwner/ScheduleField.vue')
        },
        {
          path: 'info',
          name: 'staff-info',
          component: () => import('../views/User/ProfileUser.vue')
        },
        {
          path: 'update-field/:id',
          name: 'update-field-by-staff',
          component: () => import('../views/FieldOwner/UpdateField.vue')
        },
        {
          path: 'find-booking',
          name: 'staff-find-booking',
          component: () => import('../views/FieldOwner/FindBooking.vue')
        },
        {
          path: 'posts',
          name: 'staff-posts',
          component: () => import('../views/FieldOwner/PostList.vue')
        },
        {
          path: 'post-create',
          name: 'staff-post-create',
          component: () => import('../views/FieldOwner/PostCreating.vue')
        },
        {
          path: 'post/:id',
          name: 'staff-post-detail',
          component: () => import('../views/FieldOwner/PostBlogDetail.vue')
        }
      ]
    },
    {
      path: '/admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      beforeEnter: (to, from) => {
        const authStore = useAuthStore()
        if (authStore.identity && authStore.identity.roleName == 'Admin') {
          return true
        }
        return { name: 'home' }
      },
      children: [
        {
          path: 'home',
          name: 'admin-home',
          component: () => import('../views/Admin/HomeAdmin.vue')
        },
        {
          path: 'fields',
          name: 'admin-fields',
          component: () => import('../views/Admin/fields/AdminFields.vue')
        },
        {
          path: 'fields/:id',
          name: 'field-edit',
          component: () => import('../views/Admin/fields/FieldEdit.vue')
        },
        {
          path: 'create-field',
          name: 'create-field',
          component: () => import('../views/Admin/fields/FieldCreate.vue')
        },
        {
          path: 'reports',
          name: 'list-reports',
          component: () => import('../views/Admin/fields/ReportField.vue')
        },
        {
          path: 'users',
          name: 'users',
          component: () => import('../views/Admin/users/Users.vue')
        },
        {
          path: 'user/:id',
          name: 'user-detail',
          component: () => import('../views/Admin/users/UserDetail.vue')
        },
        {
          path: 'posts',
          name: 'admin-posts',
          component: () => import('../views/Admin/posts/BlogPosts.vue')
        },
        {
          path: 'post-create',
          name: 'admin-posts-create',
          component: () => import('../views/Admin/posts/BlogPostCreate.vue')
        },
        {
          path: 'post/:id',
          name: 'admin-posts-detail',
          component: () => import('../views/Admin/posts/BlogPostDetail.vue')
        },
        {
          path: 'statistic',
          name: 'admin-statistic',
          component: () => import('../views/Admin/statistics/LayoutStatistic.vue'),
          children: [
            {
              path: 'users',
              name: 'user-statistic',
              component: () => import('../views/Admin/statistics/UserStatistic.vue')
            },
            {
              path: 'bookings',
              name: 'booking-statistic',
              component: () => import('../views/Admin/statistics/BookingStatistic.vue')
            }
          ]
        }
      ]
    },
    {
      path: '/404',
      name: '404',
      component: () => import('@/views/Page404.vue')
    },
    {
      path: '/verify-token/:token/:userId',
      name: 'verify-token',
      component: () => import('@/views/Auth/VerifyTokenPassword.vue')
    },
    {
      path: '/active/:token/:userId',
      name: 'active-account',
      component: () => import('@/views/Auth/VerifyRegister.vue')
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: () => import('@/views/Auth/ResetPassword.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/404'
    }
  ],
  scrollBehavior(to, from, savedPosition) {
    return { top: 0 }
  }
})

router.beforeEach(async (to, from) => {
  const authStore = useAuthStore()

  if (Cookies.get(ACCESS_TOKEN_KEY)) {
    await authStore.getMe()
    if (authStore.identity && to.name == 'home') {
      switch (authStore.identity.roleName) {
        case 'Player':
          return { name: 'user-home' }
        case 'Field':
          return { name: 'owner-home' }
        case 'Admin':
          return { name: 'admin-home' }
        case 'Staff':
          return { name: 'staff-home' }
      }
    }
  }

  if (!authStore.identity && authStore.identified && to.name !== 'home') {
    console.log('no identity')
    return { name: 'home' }
  }
})

export default router
