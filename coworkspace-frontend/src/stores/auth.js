import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        accessToken: null,
        expiration: null,
        isAuthenticated: false,
    }),
    getters: {
        isTokenValid: (state) => {
            if (!state.expiration) return false;
            return new Date(state.expiration) > new Date();
        },
    },
    actions: {
        async login(credentials) {
            try {
                const response = await api.login(credentials);
                this.accessToken = response.data.token;
                this.expiration = response.data.expiration;
                this.isAuthenticated = true;
                return response.data.message;
            } catch (error) {
                throw error.response?.data?.message || 'Error al iniciar sesión';
            }
        },
        logout() {
            this.accessToken = null;
            this.expiration = null;
            this.isAuthenticated = false;
        },
    },
    persist: {
        storage: sessionStorage,
    },
});