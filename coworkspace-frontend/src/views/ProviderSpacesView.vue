<template>
  <div class="container py-4">
    <h1 class="my-4 text-primary">Espacios de {{ authStore.userName }}</h1>

    <!-- Barra de búsqueda -->
    <div class="input-group mb-2">
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
    <div v-else-if="filteredSpaces.length === 0" class="text-muted">No se encontraron resultados.</div>

    <div class="row g-3" style="max-height: 70vh; overflow-y: auto;">
      <div class="col-md-6 col-lg-4" v-for="space in paginatedSpaces" :key="space.id">
        <SpaceCard :space="space">
          <template #actions>
            <div class="d-flex gap-2 mt-2">
              <button class="btn btn-sm btn-outline-primary">Ver Reservas</button>
              <button class="btn btn-sm btn-outline-secondary">Editar</button>
            </div>
          </template>
        </SpaceCard>
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

    <!-- Modal Nuevo Espacio -->
    <div class="modal fade" id="newSpaceModal" tabindex="-1" aria-labelledby="newSpaceModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <form @submit.prevent="createSpace">
            <div class="modal-header">
              <h5 class="modal-title" id="newSpaceModalLabel">Crear Nuevo Espacio</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label">Nombre</label>
                <input v-model="newSpace.name" type="text" class="form-control" required />
              </div>

              <div class="mb-3">
                <label class="form-label">Admin</label>
                <select v-model="newSpace.adminId" class="form-select" required>
                  <option value="" disabled>Seleccione un admin</option>
                  <option v-for="admin in admins" :key="admin.id" :value="admin.id">
                    {{ admin.name }}
                  </option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Ciudad</label>
                <input v-model="newSpace.city" type="text" class="form-control" required />
              </div>

              <div class="mb-3">
                <label class="form-label">Precio</label>
                <input v-model.number="newSpace.price" type="number" min="0" class="form-control" required />
              </div>

              <div class="form-check form-switch mb-3">
                <input class="form-check-input" type="checkbox" v-model="newSpace.isPublic" />
                <label class="form-check-label">Es público</label>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
              <button type="submit" class="btn btn-primary">Crear Espacio</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import { useNotyf } from '../composables/useNotyf';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';
import SpaceCard from '../components/SpaceCard.vue';
import 'notyf/notyf.min.css';

// Importa Modal explícitamente desde bootstrap
import { Modal } from 'bootstrap';

export default {
  name: 'ProviderSpacesView',
  components: { SpaceCard },
  setup() {
    const authStore = useAuthStore();
    const spaces = ref([]);
    const admins = ref([]);
    const isLoading = ref(true);
    const error = ref(null);
    const search = ref('');
    const currentPage = ref(1);
    const perPage = 6;
    const notyf = useNotyf();

    const newSpace = ref({
      name: '',
      adminId: '',
      isPublic: false,
      price: 0,
      city: ''
    });

    const fetchSpaces = async () => {
      try {
        const response = await api.getSpacesByProvider(authStore.userId);
        spaces.value = response.data;
      } catch (err) {
        error.value = err.response?.data?.message || 'Error al cargar los espacios.';
      } finally {
        isLoading.value = false;
      }
    };

    const fetchAdmins = async () => {
      try {
        const res = await api.getProviderAdmins(authStore.userId);
        admins.value = res.data;
      } catch {
        notyf.error('Error al cargar los administradores.');
      }
    };

    let modalInstance = null; // guardamos instancia modal para poder ocultarlo luego

    const openNewSpaceModal = () => {
      fetchAdmins();
      const modalEl = document.getElementById('newSpaceModal');
      modalInstance = new Modal(modalEl);
      modalInstance.show();
    };

    const createSpace = async () => {
      try {
        await api.createSpace(authStore.userId, newSpace.value);
        notyf.success('Espacio creado correctamente');
        await fetchSpaces();
        if (modalInstance) {
          modalInstance.hide();
        }
        newSpace.value = { name: '', adminId: '', isPublic: false, price: 0, city: '' };
      } catch (err) {
        notyf.error(err.response?.data?.message || 'Error al crear el espacio');
      }
    };

    const filteredSpaces = computed(() =>
      spaces.value.filter(space =>
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
      notyf
    };
  },
};
</script>

