<template>
    <div class="py-4">
        <div class="d-flex align-items-center mt-3">
            <button class="btn btn-outline-secondary mb-3" @click="$emit('back')">
                <i class="bi bi-arrow-left"></i> Volver
            </button>
        </div>

        <GenericList :title="`Gestionar reservas - ${space?.nombre}`" :items="bookings"
            :headers="['Nombre', 'Fecha Inicio', 'Fecha Fin']" :fields="['name', 'start', 'end']" :loading="loading"
            :show-add-button="false" :show-actions="true" :show-manage="false"
            search-placeholder="Buscar reserva ..." />
    </div>
</template>

<script>
import { ref, watch } from "vue";
import GenericList from '../components/GenericList.vue';
import api from "../services/api";
import { useAuthStore } from "../stores/auth";
import { useNotyf } from "../composables/useNotyf";

export default {
    name: "AdminBookingManagement",
    components: { GenericList },
    props: { space: { type: Object, required: false, default: null } },
    setup(props) {
        const authStore = useAuthStore();
        const notyf = useNotyf();
        const bookings = ref([]);
        const loading = ref(false);

        const formatDateTime = (iso) => {
            if (!iso) return "";
            try {
                return new Date(iso).toLocaleString('es-ES', {
                    dateStyle: 'short',
                    timeStyle: 'short',
                    timeZone: 'Europe/Madrid'
                });
            } catch {
                return iso;
            }
        };

        const loadBookings = async (spaceId) => {
            if (!spaceId) return;
            loading.value = true;
            try {
                const res = await api.getSpaceBookings(authStore.userId, spaceId);
                // mapear la respuesta a lo que espera GenericList
                bookings.value = (res.data || []).map(b => ({
                    id: b.id,                       // clave para v-for / idKey
                    name: b.nombreCliente || '',    // campo mostrado como 'Nombre'
                    start: formatDateTime(b.fechaInicio),
                    end: formatDateTime(b.fechaFin),
                    // si necesitas mantener los datos originales puedes añadir `raw: b`
                }));
            } catch (err) {
                notyf.error(err.response?.data?.message || "Error al cargar reservas");
                bookings.value = [];
            } finally {
                loading.value = false;
            }
        };

        watch(() => props.space, (s) => {
            if (s) loadBookings(s.id);
            else bookings.value = [];
        }, { immediate: true });

        return { bookings, loading };
    },
};
</script>
