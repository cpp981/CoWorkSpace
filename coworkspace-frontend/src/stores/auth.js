import { defineStore } from 'pinia';
import { jwtDecode } from 'jwt-decode';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    accessToken: null,
    expiration: null,
    isAuthenticated: false,
    userId: null,
    roleId: null,
  }),
  getters: {
    isTokenValid: (state) => {
      if (!state.expiration) return false;
      return new Date(state.expiration) > new Date();
    },
    userRole: (state) => {
      const roleMap = {
        '1': 'SuperAdmin',
        '2': 'Admin',
        '3': 'Provider',
        '4': 'Client',
      };
      return state.roleId ? roleMap[state.roleId] : null;
    },
  },
  actions: {
    async login(credentials) {
      try {
        const response = await api.login(credentials);
        this.accessToken = response.data.token;
        this.expiration = response.data.expiration;
        this.isAuthenticated = true;

        // Decodificar token
        const decoded = jwtDecode(this.accessToken);
        console.log('Decoded JWT:', decoded); // Temporal para depuración
        this.userId = decoded.sub;
        this.roleId = decoded.roleId;

        return response.data.message;
      } catch (error) {
        throw error.response?.data?.message || 'Error al iniciar sesión';
      }
    },
    logout() {
      this.accessToken = null;
      this.expiration = null;
      this.userId = null;
      this.roleId = null;
      this.isAuthenticated = false;
    },
  },
  persist: {
    storage: sessionStorage,
  },
});