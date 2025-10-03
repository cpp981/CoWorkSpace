<template>
    <div class="container py-4">
        <h1 class="my-4 text-primary">Espacios de {{ authStore.userName }}</h1>
        <div v-if="spaces.length" class="row g-3">

            <!-- Barra de búsqueda -->
            <div class="input-group mb-3">
                <span class="input-group-text bg-white border-end-0 border-secondary">
                    <i class="bi bi-search text-muted"></i>
                </span>
                <input v-model="search" type="text" class="form-control border-start-0 border-secondary"
                    placeholder="Buscar por nombre o ciudad..." />
            </div>
            <div class="col-md-4" v-for="space in filteredSpaces" :key="space.id">
                <AdminSpaceCard :space="space" @view-calendar="handleViewCalendar" />
            </div>
        </div>

        <p v-else class="text-muted">No tienes espacios registrados.</p>

    </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import { useAuthStore } from "../stores/auth";
import api from "../services/api";
import AdminSpaceCard from "../components/AdminSpaceCard.vue";
import { useNotyf } from "../composables/useNotyf";

export default {
    components: { AdminSpaceCard },
    setup() {
        const authStore = useAuthStore();
        const spaces = ref([]);
        const search = ref("");
        const notyf = useNotyf();

        const filteredSpaces = computed(() =>
            spaces.value.filter(
                (space) =>
                    space.nombre.toLowerCase().includes(search.value.toLowerCase()) ||
                    space.ciudad.toLowerCase().includes(search.value.toLowerCase())
            )
        );

        const handleViewCalendar = (space) => {
            console.log("Ver reservas de:", space);
        };

        onMounted(async () => {
            try {
                const res = await api.getAdminSpaces(authStore.userId);
                spaces.value = res.data || [];
            } catch (err) {
                const errors = err.response?.data?.errors;

                if (errors) {
                    for (const key in errors) {
                        errors[key].forEach((message) => notyf.error(message));
                    }
                } else {
                    notyf.error(err.response?.data?.message || "Error al cargar los espacios");
                }
            }
        });

        return {
            authStore,
            spaces,
            search,
            filteredSpaces,
            handleViewCalendar,
        };
    },
};
</script>
