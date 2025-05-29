<template>
  <Register
    title="Registrar Administrador"
    :fields="fields"
    submit-button-text="Crear Administrador"
    :submit-handler="handleRegisterAdmin"
    @cancel="$emit('cancel')"
  />
</template>

<script setup>
import { ref, computed } from 'vue';
import Register from '../components/Register.vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

defineEmits(['cancel']);

const authStore = useAuthStore();

const fields = ref([
  {
    key: 'email',
    label: 'Email',
    type: 'email',
    placeholder: 'admin@email.com',
    icon: 'bi bi-person me-1',
    required: true,
  },
  {
    key: 'password',
    label: 'Contraseña',
    type: 'password',
    placeholder: '••••••••',
    icon: 'bi bi-lock me-1',
    required: true,
  },
  {
    key: 'confirmPassword',
    label: 'Repite la contraseña',
    type: 'password',
    placeholder: '••••••••',
    icon: 'bi bi-lock me-1',
    required: true,
  },
  {
    key: 'name',
    label: 'Nombre',
    type: 'text',
    placeholder: 'Nombre del administrador',
    icon: 'bi bi-person-circle me-1',
    required: true,
  },
]);

const providerId = computed(() => parseInt(authStore.user?.userId));

const handleRegisterAdmin = async (form) => {
  if (!providerId.value) {
    throw new Error('No se encontró el ID del proveedor');
  }
  return await api.registerAdmin(providerId.value, {
    email: form.email,
    password: form.password,
    name: form.name,
    roleId: 2, // Admin
  });
};
</script>