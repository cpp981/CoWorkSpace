import axios from 'axios';

const apiClient = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

export default {
    testConnection() {
        console.log('Enviando petición a /api/test...');
        return apiClient.get('/test');
    },
};