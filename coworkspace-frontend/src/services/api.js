import axios from 'axios';
import { useAuthStore } from '../stores/auth';

const apiClient = axios.create({
    baseURL: '/api/v1',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor para añadir el access token
apiClient.interceptors.request.use((config) => {
    const authStore = useAuthStore();
    if (authStore.accessToken && authStore.isTokenValid) {
        config.headers.Authorization = `Bearer ${authStore.accessToken}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

export default {
    login(credentials) {
        return apiClient.post('/auth/login', credentials);
    },
    register(userData) {
        return apiClient.post('/auth/register', userData);
    },
    client: apiClient, // Para interceptores en App.vue
};