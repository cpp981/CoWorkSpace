<template>
  <GenericModal
    v-model="visible"
    title="Editar reserva"
    confirmText="Actualizar"
    @submit="onSubmit"
  >
    <div class="mb-3">
      <label class="form-label">Fecha inicio</label>
      <input
        v-model="localStart"
        type="datetime-local"
        class="form-control"
        required
      />
    </div>

    <div class="mb-3">
      <label class="form-label">Fecha fin</label>
      <input
        v-model="localEnd"
        type="datetime-local"
        class="form-control"
        required
        :min="localStart || minDateTime"
      />
    </div>
  </GenericModal>
</template>

<script setup>
import { ref, watch } from "vue";
import GenericModal from "../components/GenericModal.vue";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";
import { useNotyf } from "../composables/useNotyf";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  booking: { type: Object, required: true },
  spaceId: { type: [Number, String], required: true },
});

const emit = defineEmits(["update:modelValue", "updated"]);

const authStore = useAuthStore();
const notyf = useNotyf();

/* visible */
const visible = ref(!!props.modelValue);
watch(
  () => props.modelValue,
  (v) => (visible.value = !!v)
);
watch(visible, (v) => emit("update:modelValue", v));

/* campos locales para datetime-local */
const localStart = ref("");
const localEnd = ref("");
const originalIsoStart = ref(null);
const minDateTime = new Date().toISOString().slice(0, 16);
let saving = ref(false);

const toLocalInput = (iso) => {
  if (!iso) return "";
  const d = new Date(iso);
  const tzOffset = d.getTimezoneOffset() * 60000;
  const local = new Date(d.getTime() - tzOffset);
  return local.toISOString().slice(0, 16);
};
const localToIso = (localStr) => {
  if (!localStr) return null;
  const d = new Date(localStr);
  return new Date(d.getTime()).toISOString();
};

/* Inicializar campos cuando cambia booking */
watch(
  () => props.booking,
  (b) => {
    if (!b) {
      localStart.value = "";
      localEnd.value = "";
      originalIsoStart.value = null;
      return;
    }
    const rawStart = b._rawFechaInicio || b.raw?.fechaInicio || b.start || null;
    const rawEnd = b._rawFechaFin || b.raw?.fechaFin || b.end || null;

    originalIsoStart.value = rawStart ? new Date(rawStart).toISOString() : null;
    localStart.value = rawStart
      ? toLocalInput(rawStart)
      : toLocalInput(b.start);
    localEnd.value = rawEnd ? toLocalInput(rawEnd) : toLocalInput(b.end);
  },
  { immediate: true }
);

const onSubmit = async () => {
  if (saving.value) return;

  const isoStart = localToIso(localStart.value);
  const isoEnd = localToIso(localEnd.value);

  if (!isoStart || !isoEnd) {
    notyf.error("Fechas inválidas.");
    return;
  }

  // Detectar si el usuario modificó start respecto al original
  const startWasModified = originalIsoStart.value
    ? new Date(isoStart).toISOString() !==
      new Date(originalIsoStart.value).toISOString()
    : true; // si no había original, consideramos modificado

  if (startWasModified) {
    const todayStart = new Date();
    todayStart.setHours(0, 0, 0, 0);
    if (new Date(isoStart) < todayStart) {
      notyf.error(
        "La fecha de inicio no puede ser anterior al día de hoy si la modificas."
      );
      return;
    }
  }

  // Comprobación start < end
  if (new Date(isoStart) >= new Date(isoEnd)) {
    notyf.error("La fecha de inicio debe ser anterior a la fecha de fin.");
    return;
  }

  saving.value = true;
  try {
    const payload = { Start: isoStart, End: isoEnd };
    const res = await api.updateBooking(
      authStore.userId,
      props.spaceId,
      props.booking.id,
      payload
    );

    const msg =
      res?.data?.Message ||
      res?.data?.message ||
      "Reserva actualizada correctamente.";
    notyf.success(msg);

    // Emitir evento para que el padre recargue/actualice la lista
    emit("updated", {
      bookingId: props.booking.id,
      start: isoStart,
      end: isoEnd,
      response: res.data,
    });

    visible.value = false; // cierra modal
  } catch (err) {
    const errMsg =
      err.response?.data?.Message ||
      err.response?.data?.message ||
      "Error al actualizar reserva";
    notyf.error(errMsg);
  } finally {
    saving.value = false;
  }
};
</script>
