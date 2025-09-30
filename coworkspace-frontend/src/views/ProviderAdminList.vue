<template>
    <GenericList :title="`Admins para ${authStore.userName}`" :items="admins" :headers="['ID', 'Nombre']"
        :fields="['id', 'name']" :loading="loading" addLabel="Nuevo Administrador" :showAddButton="true"
        :showActions="true" @add="handleAdd" @edit="handleEdit" @delete="handleDelete" />

</template>

<script>
import { ref, onMounted } from "vue";
import GenericList from "../components/GenericList.vue";
import api from "../services/api";
import { useNotyf } from "../composables/useNotyf";
import { useAuthStore } from "../stores/auth";

export default {
    name: "ProviderAdminList",
    components: { GenericList },
    setup() {
        const admins = ref([]);
        const loading = ref(false);
        const notyf = useNotyf();
        const authStore = useAuthStore();

        const fetchAdmins = async () => {
            loading.value = true;
            try {
                const res = await api.getProviderAdmins();
                admins.value = res.data;
            } catch {
                notyf.error("Error al cargar administradores");
            } finally {
                loading.value = false;
            }
        };

        const handleAdd = () => console.log("Agregar admin");
        const handleEdit = (admin) => console.log("Editar", admin);
        const handleDelete = (admin) => console.log("Eliminar", admin);

        onMounted(fetchAdmins);

        return { admins, loading, authStore, handleAdd, handleEdit, handleDelete };
    },
};
</script>
