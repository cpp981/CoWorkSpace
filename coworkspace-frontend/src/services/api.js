import axios from 'axios';
import { useAuthStore } from '../stores/auth';

const apiClient = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
});

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
  registerAdmin(providerId, adminData) {
    return apiClient.post(`/providers/${providerId}/admins`, adminData);
  },
  getSuperAdminStats(userId) {
    return apiClient.get(`/stats/superadmin/${userId}`);
  },
  getAdminStats(userId) {
    return apiClient.get(`/stats/admin/${userId}`);
  },
  getProviderStats(userId) {
    return apiClient.get(`/stats/provider/${userId}`);
  },
  getClientStats(userId) {
    return apiClient.get(`/stats/client/${userId}`);
  },
  getMenu() {
    return apiClient.get(`/menu`);
  },
  getSpacesByProvider(providerId) {
    return apiClient.get(`/providers/${providerId}/spaces/list`);
  },
createSpace(providerId, spaceData) {
  return apiClient.post(`/providers/${providerId}/spaces/create`, spaceData);
},
getProviderAdmins(providerId) {
  return apiClient.get(`/providers/${providerId}/admins`);
},
getBookingsBySpace(spaceId){
  return apiClient.get(`/spaces/${spaceId}/bookings`)
},
updateSpace(providerId, id, data) {
    return apiClient.put(`/providers/${providerId}/spaces/${id}/update`, data);
},
deleteSpace(providerId, id){
  return apiClient.delete(`/providers/${providerId}/spaces/${id}/delete`);
},
  client: apiClient,
};