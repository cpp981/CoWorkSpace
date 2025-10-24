<template>
  <div class="py-4">
    <div class="d-flex align-items-center mt-3">
      <button class="btn btn-outline-secondary mb-3" @click="$emit('back')">
        <i class="bi bi-arrow-left"></i> Volver
      </button>
    </div>

    <!-- Lista de reservas -->
    <GenericList
      :title="`Gestionar reservas - ${space?.nombre}`"
      :items="bookings"
      :headers="['Nombre', 'Fecha Inicio', 'Fecha Fin']"
      :fields="['name', 'start', 'end']"
      :loading="loading"
      :show-add-button="false"
      :show-actions="true"
      :show-manage="false"
      @edit="handleEdit"
      @delete="handleDelete"
      search-placeholder="Buscar reserva ..."
    />

    <!-- Modal de edición -->
    <BookingEditModal
      v-model="showEditModal"
      :booking="editingBooking"
      :space-id="space?.id"
      @updated="onBookingUpdated"
    />

    <!-- Modal de confirmación de borrado -->
    <ConfirmDeleteModal
      v-model="showDeleteModal"
      title="Borrar Reserva"
      :message="`Se va a eliminar la reserva de <strong>'${clientBookingToDelete?.name}'</strong>.<br>¿Estás seguro?`"
      @confirm="deleteBooking"
    />
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import GenericList from "../components/GenericList.vue";
import BookingEditModal from "../components/BookingEditModal.vue";
import ConfirmDeleteModal from "../components/ConfirmDeleteModal.vue";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";
import { useNotyf } from "../composables/useNotyf";

const props = defineProps({
  space: { type: Object, required: false },
});

const authStore = useAuthStore();
const notyf = useNotyf();

const bookings = ref([]);
const loading = ref(false);

/* Estado para el modal de edición */
const showEditModal = ref(false);
const editingBooking = ref(null);

const showDeleteModal = ref(false);
const clientBookingToDelete = ref(false);
const spaceIdBooking = ref(false);

const formatDateTime = (iso) => {
  if (!iso) return "";
  try {
    return new Date(iso).toLocaleString("es-ES", {
      dateStyle: "short",
      timeStyle: "short",
      timeZone: "Europe/Madrid",
    });
  } catch {
    return iso;
  }
};

/* Carga reservas del espacio y preserva las fechas raw para reabrir el modal */
const loadBookings = async (spaceId) => {
  if (!spaceId) {
    bookings.value = [];
    return;
  }
  loading.value = true;
  try {
    const res = await api.getSpaceBookings(authStore.userId, spaceId);
    bookings.value = (res.data || []).map((b) => ({
      id: b.id,
      name: b.nombreCliente || "",
      start: formatDateTime(b.fechaInicio),
      end: formatDateTime(b.fechaFin),
      _rawFechaInicio: b.fechaInicio,
      _rawFechaFin: b.fechaFin,
      raw: b,
    }));
  } catch (err) {
    notyf.error(err.response?.data?.message || "Error al cargar reservas");
    bookings.value = [];
  } finally {
    loading.value = false;
  }
};

/* Abrir modal para editar (GenericList emite el item como row) */
const handleEdit = (row) => {
  editingBooking.value = row;
  showEditModal.value = true;
};

const handleDelete = async (row) => {
  clientBookingToDelete.value = row;
  showDeleteModal.value = true;
  spaceIdBooking.value = props.space.id;
};

const deleteBooking = async () => {
  // Llamamos al endpoint para borrar la reserva
  try {
    const res = await api.deleteBooking(
      authStore.userId,
      spaceIdBooking.value,
      clientBookingToDelete.value.id
    );
    // Actualizamos la lista si el borrado es correcto
    loadBookings(spaceIdBooking.value);
    notyf.success(res.data.message);
  } catch (error) {
    notyf.error(error.response?.data?.message || "Error al borrar la reserva");
  }
};

/* Cuando BookingEditModal emite 'updated', recargamos la lista */
const onBookingUpdated = async () => {
  await loadBookings(props.space?.id);
};

/* Recargamos cuando cambie la prop space */
watch(
  () => props.space,
  (s) => {
    if (s && s.id) loadBookings(s.id);
    else bookings.value = [];
  },
  { immediate: true }
);
</script>
