<template>
  <div class="container py-4">
    <h1 class="my-4 text-primary">Espacios de {{ authStore.userName }}</h1>

    <!-- Barra de búsqueda -->
    <div class="input-group mb-3">
      <span class="input-group-text bg-white border-end-0 border-secondary">
        <i class="bi bi-search text-muted"></i>
      </span>
      <input v-model="search" type="text" class="form-control border-start-0 border-secondary"
        placeholder="Buscar por nombre o ciudad..." />
    </div>

    <!-- Botón Nuevo Espacio -->
    <div class="d-flex justify-content-end mb-3">
      <button class="btn btn-success" @click="openNewSpaceModal">
        <i class="bi bi-plus-lg"></i> Nuevo Espacio
      </button>
    </div>

    <!-- Lista de espacios -->
    <div v-if="isLoading" class="text-muted">Cargando espacios...</div>
    <div v-else-if="filteredSpaces.length === 0" class="text-muted">
      No se encontraron resultados.
    </div>

    <div class="row g-3" style="max-height: 70vh; overflow-y: auto">
      <div class="col-md-6 col-lg-4" v-for="space in paginatedSpaces" :key="space.id">
        <SpaceCard :space="space" @view-bookings="handleViewBookings" @edit-space="handleEditSpace"
          @delete-space="handleDeleteSpace" />
      </div>
    </div>

    <!-- Paginación -->
    <nav class="mt-3 d-flex justify-content-center" v-if="totalPages > 1">
      <ul class="pagination">
        <li class="page-item" :class="{ disabled: currentPage === 1 }">
          <button class="page-link" @click="prevPage">Anterior</button>
        </li>
        <li class="page-item" v-for="n in totalPages" :key="n" :class="{ active: currentPage === n }">
          <button class="page-link" @click="goToPage(n)">{{ n }}</button>
        </li>
        <li class="page-item" :class="{ disabled: currentPage === totalPages }">
          <button class="page-link" @click="nextPage">Siguiente</button>
        </li>
      </ul>
    </nav>
    <!--Modal para confirmar el borrado-->
    <ConfirmDeleteModal v-model="showDeleteModal" title="Borrar Espacio"
      :message="`¿Estás seguro de borrar '${spaceToDelete?.name}'?`" @confirm="deleteSpace" />
    <!-- Modal Editar Espacio -->
    <GenericModal v-model="showEditSpaceModal" title="Editar Espacio" confirmText="Guardar cambios" @submit="editSpace">
      <div v-if="selectedSpace">
        <div class="mb-3">
          <label class="form-label">Nombre del Espacio</label>
          <input v-model="selectedSpace.name" type="text" class="form-control border-secondary" required />
        </div>
        <div class="mb-3">
          <label class="form-label">Administrador</label>
          <select v-model="selectedSpace.adminId" class="form-select border-secondary" required>
            <option value="" disabled>Seleccione un administrador</option>
            <option v-for="admin in admins" :key="admin.id" :value="admin.id">
              {{ admin.name }}
            </option>
          </select>
        </div>
        <div class="mb-3">
          <label class="form-label">Ciudad</label>
          <input v-model="selectedSpace.city" type="text" class="form-control border-secondary" required />
        </div>
        <div class="mb-3">
          <label class="form-label">Precio</label>
          <input v-model.number="selectedSpace.price" type="number" min="0" class="form-control border-secondary"
            required />
        </div>
        <div class="form-check form-switch mb-3">
          <input class="form-check-input border-secondary" type="checkbox" v-model="selectedSpace.isPublic" />
          <label class="form-check-label">Es público</label>
        </div>
      </div>
    </GenericModal>

    <!--Modal Crear Espacio-->
    <GenericModal v-model="showNewSpaceModal" title="Crear Nuevo Espacio" confirmText="Crear Espacio"
      @submit="createSpace">
      <div class="mb-3">
        <label class="form-label">Nombre del Espacio</label>
        <input v-model="newSpace.name" type="text" class="form-control border-secondary" required />
      </div>
      <div class="mb-3">
        <label class="form-label">Administrador</label>
        <select v-model="newSpace.adminId" class="form-select border-secondary" required>
          <option value="" disabled>Seleccione un administrador</option>
          <option v-for="admin in admins" :key="admin.id" :value="admin.id">
            {{ admin.name }}
          </option>
        </select>
      </div>
      <div class="mb-3">
        <label class="form-label">Ciudad</label>
        <input v-model="newSpace.city" type="text" class="form-control border-secondary" required />
      </div>
      <div class="mb-3">
        <label class="form-label">Precio</label>
        <input v-model.number="newSpace.price" type="number" min="0" class="form-control border-secondary" required />
      </div>
      <div class="form-check form-switch mb-3">
        <input class="form-check-input border-secondary" type="checkbox" v-model="newSpace.isPublic" />
        <label class="form-check-label">Es público</label>
      </div>
    </GenericModal>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import { useNotyf } from "../composables/useNotyf";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";
import SpaceCard from "../components/SpaceCard.vue";
import GenericModal from "../components/GenericModal.vue";
import ConfirmDeleteModal from "./ConfirmDeleteModal.vue";
import "notyf/notyf.min.css";
import { Modal } from "bootstrap";

export default {
  name: "ProviderSpacesView",
  components: { SpaceCard, GenericModal, ConfirmDeleteModal },
  emits: ["view-bookings"],
  emits: ["edit-space"],
  emits: ["delete-space"],
  setup(props, context) {
    const authStore = useAuthStore();
    const spaces = ref([]);
    const admins = ref([]);
    const isLoading = ref(true);
    const search = ref("");
    const currentPage = ref(1);
    const perPage = 6;
    const notyf = useNotyf();

    const newSpace = ref({
      name: "",
      adminId: "",
      isPublic: false,
      price: 0,
      city: "",
    });

    const fetchSpaces = async () => {
      try {
        const response = await api.getSpacesByProvider(authStore.userId);
        spaces.value = response.data || [];
      } catch (err) {
        notyf.error("Error al cargar los espacios");
      } finally {
        isLoading.value = false;
      }
    };

    const fetchAdmins = async () => {
      try {
        const res = await api.getProviderAdmins(authStore.userId);
        admins.value = res.data;
      } catch {
        notyf.error("Error al cargar los administradores.");
      }
    };

    const showNewSpaceModal = ref(false);

    const openNewSpaceModal = async () => {
      await fetchAdmins();
      showNewSpaceModal.value = true;
    };

    const createSpace = async () => {
      try {
        await api.createSpace(authStore.userId, newSpace.value);
        notyf.success("Espacio creado correctamente");
        await fetchSpaces();
        showNewSpaceModal.value = false; // cerrar modal de forma reactiva
        // reset
        newSpace.value = { name: "", adminId: "", city: "", price: 0, isPublic: false };
      } catch (err) {
        notyf.error(err.response?.data?.message || "Error al crear el espacio");
      }
    };

    const handleViewBookings = (space) => {
      context.emit("view-bookings", space);
    };

    const showEditSpaceModal = ref(false);   // control del modal
    const selectedSpace = ref(null);

    const handleEditSpace = async (space) => {
      selectedSpace.value = { ...space };
      await fetchAdmins();
      showEditSpaceModal.value = true; // abrir modal de forma reactiva
    };

    const editSpace = async () => {
      try {
        const response = await api.updateSpace(
          authStore.userId,
          selectedSpace.value.id, // usamos el id del espacio
          {
            name: selectedSpace.value.name,
            adminId: selectedSpace.value.adminId,
            isPublic: selectedSpace.value.isPublic,
            price: selectedSpace.value.price,
            city: selectedSpace.value.city,
          }
        );

        if (response.data.success) {
          notyf.success(response.data.message);
          showEditSpaceModal.value = false; // cerrar modal
          await fetchSpaces(); // refrescar lista
        } else {
          notyf.error(response.data.message);
        }
      } catch (err) {
        notyf.error("Error al editar el espacio");
        console.error(err);
      }
    };

    const showDeleteModal = ref(false);
    const spaceToDelete = ref(null);

    const handleDeleteSpace = async (space) => {
      spaceToDelete.value = space;
      showDeleteModal.value = true;
    };

    const deleteSpace = async () => {
      try {
        await api.deleteSpace(authStore.userId, spaceToDelete.value.id);
        notyf.success("Espacio eliminado correctamente");
        showDeleteModal.value = false;
        await fetchSpaces();
      } catch (err) {
        notyf.error("Error al eliminar el espacio");
        console.error(err);
      }
    };

    const filteredSpaces = computed(() =>
      spaces.value.filter(
        (space) =>
          space.name.toLowerCase().includes(search.value.toLowerCase()) ||
          space.city.toLowerCase().includes(search.value.toLowerCase())
      )
    );

    const totalPages = computed(() =>
      Math.ceil(filteredSpaces.value.length / perPage)
    );

    const paginatedSpaces = computed(() => {
      const start = (currentPage.value - 1) * perPage;
      return filteredSpaces.value.slice(start, start + perPage);
    });

    const goToPage = (n) => {
      if (n >= 1 && n <= totalPages.value) currentPage.value = n;
    };

    const nextPage = () => goToPage(currentPage.value + 1);
    const prevPage = () => goToPage(currentPage.value - 1);

    onMounted(fetchSpaces);

    return {
      authStore,
      spaces,
      admins,
      isLoading,
      search,
      filteredSpaces,
      paginatedSpaces,
      currentPage,
      totalPages,
      goToPage,
      nextPage,
      prevPage,
      openNewSpaceModal,
      createSpace,
      newSpace,
      notyf,
      handleViewBookings,
      handleEditSpace,
      selectedSpace,
      editSpace,
      showNewSpaceModal,
      showEditSpaceModal,
      handleDeleteSpace,
      showDeleteModal,
      selectedSpace,
      spaceToDelete,
      deleteSpace
    };
  },
};
</script>
