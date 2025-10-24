<template>
  <div class="container py-4">
    <h2 class="my-4 titulo">Clientes</h2>
    <GenericList
      :items="clientsForList"
      :headers="['ID', 'Nombre', 'Reserva activa']"
      :fields="['clientId', 'clientName', 'spaceNamesHtml']"
      id-key="clientId"
      :loading="loading"
      :show-add-button="false"
      :show-actions="false"
      :show-manage="true"
      @edit="habdleEdit"
      search-placeholder="Buscar cliente o espacio..."
    />
  </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import GenericList from "../components/GenericList.vue";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";
import { useNotyf } from "../composables/useNotyf";

export default {
  name: "AdminClientsList",
  components: { GenericList },
  setup() {
    const authStore = useAuthStore();
    const notyf = useNotyf();

    const clients = ref([]);
    const loading = ref(false);

    // Paleta de clases de badge bootstrap
    const badgeClasses = [
      "badge bg-primary-subtle text-primary me-1",
      "badge bg-success-subtle text-success me-1",
      "badge bg-info-subtle text-info me-1",
      "badge bg-warning-subtle text-warning me-1",
      "badge bg-danger-subtle text-danger me-1",
      "badge bg-secondary-subtle text-secondary me-1",
      "badge bg-dark-subtle text-dark me-1",
    ];

    // Determinístico: mismo nombre => mismo color
    const getBadgeClassFor = (str) => {
      if (!str) return badgeClasses[0];
      let hash = 0;
      for (let i = 0; i < str.length; i++) {
        hash = (hash << 5) - hash + str.charCodeAt(i);
        hash |= 0;
      }
      const idx = Math.abs(hash) % badgeClasses.length;
      return badgeClasses[idx];
    };

    // Transformar los datos para pasar al GenericList:
    const clientsForList = computed(() =>
      clients.value.map((c) => ({
        clientId: c.clientId,
        clientName: c.clientName,
        spaceNamesHtml: (c.spaceNames || [])
          .map(
            (s) =>
              `<span class="${getBadgeClassFor(s)}" title="${escapeHtml(
                s
              )}">${escapeHtml(s)}</span>`
          )
          .join(" "),
      }))
    );

    // Helper para escapar texto en HTML
    const escapeHtml = (unsafe) =>
      String(unsafe)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");

    const fetchClients = async () => {
      loading.value = true;
      try {
        const res = await api.getClientsByAdmin(authStore.userId);
        clients.value = res.data;
        console.log(res);
      } catch (err) {
        // Mostrar mensaje que venga del back si existe
        const msg = err?.response?.data?.message || "Error al cargar clientes";
        notyf.error(msg);
      } finally {
        loading.value = false;
      }
    };

    onMounted(fetchClients);

    return {
      authStore,
      clientsForList,
      loading,
    };
  },
};
</script>
