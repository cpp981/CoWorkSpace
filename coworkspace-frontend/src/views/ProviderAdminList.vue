<template>
    <div class="container py-4">
        <GenericList :title="`Admins para ${authStore.userName}`" :items="admins" :headers="['ID', 'Nombre']"
            :fields="['id', 'name']" :loading="loading" addLabel="Nuevo Administrador" :showAddButton="true"
            :showActions="true" @add="handleAdd" @edit="handleEdit" @delete="handleDelete" />

        <!-- Modal editar admin -->
        <GenericModal v-model="showEditAdminModal" title="Editar Administrador" confirmText="Editar"
            @submit="editAdmin">
            <div class="mb-3">
                <label class="form-label">Nombre</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-person"></i>
                    </span>
                    <input v-model="editAdmin.name" type="text" class="form-control border-dark" required />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Email</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-envelope"></i>
                    </span>
                    <input v-model="editAdmin.email" type="email" class="form-control border-dark" required />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Contraseña</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-lock"></i>
                    </span>
                    <input v-model="editAdmin.password" type="password" class="form-control border-dark" required />
                </div>
            </div>
        </GenericModal>

        <!-- Modal crear admin -->
        <GenericModal v-model="showAddModal" title="Crear Nuevo Administrador" confirmText="Crear"
            @submit="createAdmin">
            <div class="mb-3">
                <label class="form-label">Nombre</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-person"></i>
                    </span>
                    <input v-model="newAdmin.name" type="text" class="form-control border-dark" required />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Email</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-envelope"></i>
                    </span>
                    <input v-model="newAdmin.email" type="email" class="form-control border-dark" required />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Contraseña</label>
                <div class="input-group">
                    <span class="input-group-text border-dark">
                        <i class="bi bi-lock"></i>
                    </span>
                    <input v-model="newAdmin.password" type="password" class="form-control border-dark" required />
                </div>
            </div>
        </GenericModal>
    </div>
</template>

<script>
import { ref, onMounted } from "vue";
import GenericList from "../components/GenericList.vue";
import GenericModal from "../components/GenericModal.vue";
import api from "../services/api";
import { useNotyf } from "../composables/useNotyf";
import { useAuthStore } from "../stores/auth";

export default {
    name: "ProviderAdminList",
    components: { GenericList, GenericModal },
    setup() {
        const admins = ref([]);
        const loading = ref(false);
        const showAddModal = ref(false);
        const showEditAdminModal = ref(false);
        const newAdmin = ref({ name: "", email: "", password: "" });
        const editAdmin = ref({ name: "", email: "", password: "" });
        const notyf = useNotyf();
        const authStore = useAuthStore();

        const fetchAdmins = async () => {
            loading.value = true;
            try {
                const res = await api.getProviderAdmins(authStore.userId);
                admins.value = res.data;
            } catch {
                notyf.error("Error al cargar administradores");
            } finally {
                loading.value = false;
            }
        };

        const handleAdd = () => {
            // limpiar datos antes de abrir
            newAdmin.value = { name: "", email: "", password: "" };
            showAddModal.value = true;
        };

        const createAdmin = async () => {
            try {
                const response = await api.createAdmin(
                    authStore.userId,
                    {
                        Email: newAdmin.value.email,
                        Password: newAdmin.value.password,
                        Name: newAdmin.value.name,
                        RoleId: 2
                    }
                );
                // Mostrar mensaje de éxito que viene del back
                notyf.success(response.data?.message || "Administrador creado correctamente");

                showAddModal.value = false;
                await fetchAdmins();
            } catch (err) {
                const errors = err.response?.data?.errors;

                if (errors) {
                    // Recorremos todos los errores que envía ModelState
                    for (const key in errors) {
                        errors[key].forEach(message => notyf.error(message));
                    }
                } else {
                    // Mensaje genérico si no vienen errores detallados
                    notyf.error(err.response?.data?.message || "Error al crear administrador");
                }
            }
        };

        const handleEdit = (admin) => {
            editAdmin.value =
            {
                name: admin.name,
                email: admin.email,
                password: ""
            };
            showEditAdminModal.value = true;
        }

        const editAdminByProvider = async () => {

            const response = await api.editAdmin(
                authStore.userId,
                {
                    name: editAdmin.value.name,
                    email: editAdmin.value.email,
                    password: editAdmin.value.password,
                }
            );
        }
        const handleDelete = (admin) => console.log("Eliminar", admin);

        onMounted(fetchAdmins);

        return {
            admins,
            loading,
            authStore,
            handleAdd,
            handleEdit,
            handleDelete,
            showAddModal,
            showEditAdminModal,
            newAdmin,
            editAdmin,
            editAdminByProvider,
            createAdmin,
        };
    },
};
</script>