import axios from 'axios';

const apiClient = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

export default {
   login(credentials) {
        return apiClient.post('/auth/login', credentials);
    },
    register(userData) {
        return apiClient.post('/auth/register', userData);
    },
};