<template>
  <GenericModal
    v-model="internalShow"
    :title="title"
    confirmText="Borrar"
    @submit="handleConfirm"
  >
    <p v-html="message" />
  </GenericModal>
</template>

<script setup>
import { computed } from "vue";
import GenericModal from "./GenericModal.vue";

const props = defineProps({
  modelValue: { type: Boolean, required: true },
  title: { type: String, default: null },
  message: { type: String, required: true },
});

const emit = defineEmits(["update:modelValue", "confirm"]);

// Computed con getter y setter para exponer v-model
const internalShow = computed({
  get: () => props.modelValue,
  set: (val) => emit("update:modelValue", val),
});

const handleConfirm = () => {
  emit("confirm"); // avisamos al padre
  emit("update:modelValue", false); // cerramos el modal
};
</script>
