<template>
  <Register
    title="Registrar Administrador"
    :fields="fields"
    submit-button-text="Crear Administrador"
    :submit-handler="handleRegisterAdmin"
    @cancel="$emit('cancel')"
  />
</template>

<script>
import Register from '../components/Register.vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

export default {
  name: 'ProviderRegisterAdmin',
  components: { Register },
  setup() {
    const authStore = useAuthStore();
    return { authStore };
  },
  data() {
    return {
      fields: [
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
      ],
    };
  },
  computed: {
    providerId() {
      return parseInt(this.authStore.user?.userId);
    },
  },
  methods: {
    async handleRegisterAdmin(form) {
      if (!this.providerId) {
        throw new Error('No se encontró el ID del proveedor');
      }
      return await api.registerAdmin(this.providerId, {
        email: form.email,
        password: form.password,
        name: form.name,
        roleId: 2, // Admin
      });
    },
  },
  emits: ['cancel'],
};
</script>