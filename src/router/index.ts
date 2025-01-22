import { createRouter, createWebHistory } from 'vue-router';
import Login from '@/views/Login.vue';
import Register from '@/views/Register.vue';
import Notes from '@/views/NotesPage.vue';

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/notes', component: Notes },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
