import { createApp } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
import App from './App.vue';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

// Import các components
import Home from './views/Home.vue';
import NotFound from './views/NotFound.vue';
import Redirect from './views/Redirect.vue';
import UrlStats from './views/UrlStats.vue';

// Định nghĩa routes
const routes = [
  { path: '/', component: Home },
  { path: '/stats/:code', component: UrlStats },
  { path: '/r/:code', component: Redirect },
  { path: '/:pathMatch(.*)*', component: NotFound }
];

// Tạo router
const router = createRouter({
  history: createWebHistory(),
  routes
});

// Tạo app và mount
const app = createApp(App);
app.use(router);
app.mount('#app'); 