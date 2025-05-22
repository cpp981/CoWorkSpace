import { createApp } from 'vue';
import {createPinia} from 'pinia';
import './styles/global.css'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-icons/font/bootstrap-icons.css';

const app = createApp(App);
const pinia = createPinia();
app.use(pinia);
app.mount('#app');
