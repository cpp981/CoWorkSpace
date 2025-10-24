<template>
  <div class="container py-4">
    <h2 class="my-4 titulo">Administradores</h2>
    <!--  Lista de administradores -->
    <GenericList
      :items="paginatedAdmins"
      :headers="['ID', 'Nombre']"
      :fields="['id', 'name']"
      :loading="loading"
      addLabel="Nuevo Administrador"
      :showAddButton="true"
      :showActions="true"
      :showManage="false"
      @add="handleAdd"
      @edit="handleEdit"
      @delete="handleDelete"
    />

    <!-- Paginación -->
    <GenericPagination
      :currentPage="currentPage"
      :totalPages="totalPages"
      @prev="prevPage"
      @next="nextPage"
      @goToPage="goToPage"
    />

    <!-- Modal para confirmar el borrado -->
    <ConfirmDeleteModal
      v-model="showDeleteModal"
      tittle="Borrar Administrador"
      :message="`Se va a borrar a
        <strong>'${adminToDelete?.name}'</strong>,<br><br> ¿Estás seguro?`"
      @confirm="deleteAdmin"
    />

    <!-- Modal editar admin -->
    <GenericModal
      v-model="showEditAdminModal"
      title="Editar Administrador"
      confirmText="Editar"
      @submit="editAdminByProvider"
    >
      <div class="mb-3">
        <label class="form-label">Nombre</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-person"></i>
          </span>
          <input
            v-model="editAdmin.name"
            type="text"
            class="form-control border-dark"
            required
          />
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label">Email</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-envelope"></i>
          </span>
          <input
            v-model="editAdmin.email"
            type="email"
            class="form-control border-dark"
            required
          />
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label">Contraseña</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-lock"></i>
          </span>
          <input
            v-model="editAdmin.password"
            type="password"
            class="form-control border-dark"
          />
        </div>
      </div>
    </GenericModal>

    <!-- Modal crear admin -->
    <GenericModal
      v-model="showAddModal"
      title="Crear Nuevo Administrador"
      confirmText="Crear"
      @submit="createAdmin"
    >
      <div class="mb-3">
        <label class="form-label">Nombre</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-person"></i>
          </span>
          <input
            v-model="newAdmin.name"
            type="text"
            class="form-control border-dark"
            required
          />
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label">Email</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-envelope"></i>
          </span>
          <input
            v-model="newAdmin.email"
            type="email"
            class="form-control border-dark"
            required
          />
        </div>
      </div>

      <div class="mb-3">
        <label class="form-label">Contraseña</label>
        <div class="input-group">
          <span class="input-group-text border-dark">
            <i class="bi bi-lock"></i>
          </span>
          <input
            v-model="newAdmin.password"
            type="password"
            class="form-control border-dark"
            required
          />
        </div>
      </div>
    </GenericModal>
  </div>
</template>

<script>
import { ref, onMounted, computed, watch } from "vue";
import GenericList from "../components/GenericList.vue";
import GenericModal from "../components/GenericModal.vue";
import ConfirmDeleteModal from "../components/ConfirmDeleteModal.vue";
import GenericPagination from "../components/GenericPagination.vue";
import api from "../services/api";
import { useNotyf } from "../composables/useNotyf";
import { useAuthStore } from "../stores/auth";

export default {
  name: "ProviderAdminList",
  components: {
    GenericList,
    GenericModal,
    ConfirmDeleteModal,
    GenericPagination,
  },
  setup() {
    const admins = ref([]);
    const loading = ref(false);
    const showAddModal = ref(false);
    const showEditAdminModal = ref(false);
    const showDeleteModal = ref(false);
    const newAdmin = ref({ name: "", email: "", password: "" });
    const editAdmin = ref({ name: "", email: "", password: "" });
    const currentPage = ref(1);
    const perPage = 10;
    const search = ref("");
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
        const response = await api.createAdmin(authStore.userId, {
          Email: newAdmin.value.email,
          Password: newAdmin.value.password,
          Name: newAdmin.value.name,
          RoleId: 2,
        });
        // Mostrar mensaje de éxito que viene del back
        notyf.success(
          response.data?.message || "Administrador creado correctamente"
        );

        showAddModal.value = false;
        await fetchAdmins();
      } catch (err) {
        const errors = err.response?.data?.errors;

        if (errors) {
          // Recorremos todos los errores que envía ModelState
          for (const key in errors) {
            errors[key].forEach((message) => notyf.error(message));
          }
        } else {
          // Mensaje genérico si no vienen errores detallados
          notyf.error(
            err.response?.data?.message || "Error al crear administrador"
          );
        }
      }
    };

    const handleEdit = (admin) => {
      // Traer los datos antes de abrir
      editAdmin.value = {
        id: admin.id,
        name: admin.name,
        email: admin.email,
        password: "",
      };
      showEditAdminModal.value = true;
    };

    const editAdminByProvider = async () => {
      try {
        const payload = {
          name: editAdmin.value.name,
          email: editAdmin.value.email,
        };

        // Solo agregar password si tiene valor
        if (
          editAdmin.value.password &&
          editAdmin.value.password.trim() !== ""
        ) {
          payload.password = editAdmin.value.password;
        }

        const response = await api.updateAdmin(
          authStore.userId,
          editAdmin.value.id,
          payload
        );

        notyf.success(
          response.data?.message || "Administrador actualizado correctamente"
        );

        showEditAdminModal.value = false;
        await fetchAdmins();
      } catch (err) {
        const errors = err.response?.data?.errors;

        if (errors) {
          for (const key in errors) {
            errors[key].forEach((message) => notyf.error(message));
          }
        } else {
          notyf.error(
            err.response?.data?.message ||
              "Error al actualizar el administrador"
          );
        }
      }
    };
    const adminToDelete = ref(null);

    const handleDelete = async (admin) => {
      adminToDelete.value = admin;
      showDeleteModal.value = true;
    };

    const deleteAdmin = async () => {
      try {
        const response = await api.deleteAdmin(
          authStore.userId,
          adminToDelete.value.id
        );
        notyf.success(
          response.data?.message || "Administrador borrado correctamente"
        );
        showDeleteModal.value = false;
        await fetchAdmins();
      } catch (err) {
        const errors = err.response?.data?.errors;

        if (errors) {
          for (const key in errors) {
            errors[key].forEach((message) => notyf.error(message));
          }
        } else {
          notyf.error(
            err.response?.data?.message || "Error al borrar el administrador"
          );
        }
      }
    };

    // Paginación
    const filteredAdmins = computed(() =>
      admins.value.filter((admin) =>
        admin.name.toLowerCase().includes(search.value.toLowerCase())
      )
    );
    const totalPages = computed(() =>
      Math.ceil(filteredAdmins.value.length / perPage)
    );

    watch(totalPages, (newVal) => {
      if (currentPage.value > newVal) {
        currentPage.value = newVal > 0 ? newVal : 1;
      }
    });

    const paginatedAdmins = computed(() => {
      const start = (currentPage.value - 1) * perPage;
      return filteredAdmins.value.slice(start, start + perPage);
    });

    const goToPage = (n) => {
      if (n >= 1 && n <= totalPages.value) currentPage.value = n;
    };

    const nextPage = () => goToPage(currentPage.value + 1);
    const prevPage = () => goToPage(currentPage.value - 1);

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
      showDeleteModal,
      newAdmin,
      editAdmin,
      editAdminByProvider,
      createAdmin,
      deleteAdmin,
      adminToDelete,
      currentPage,
      totalPages,
      goToPage,
      nextPage,
      prevPage,
      paginatedAdmins,
      search,
    };
  },
};
</script>
