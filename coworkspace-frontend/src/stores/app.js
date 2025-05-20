import {defineStore} from 'pinia';
import api from '../services/api';

export const useAppStore = defineStore('app', {
    state: () => ({
        testMessage: null,
        loading: false,
        error: null,
    }),
    actions: {
        async testConnection(){
            console.log('Ejecutando testConnection en el store...');
            this.loading = true;
            this.error = null;
            try{
                const response = await api.testConnection();
                this.testMessage = response.data.message;
            } catch(err){
                this.error = 'Error al conectar con el backend';
                console.error(err);
            }finally {
                this.loading = false;
            }
        },
    }, 
});