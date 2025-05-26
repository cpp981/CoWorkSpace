<template>
  <Register
    title="Registrarse"
    :fields="fields"
    submit-button-text="Registrarse"
    :submit-handler="handleRegister"
    @cancel="$emit('cancel')"
  />
</template>

<script>
import Register from '../components/Register.vue';
import api from '../services/api';

export default {
  name: 'UserRegister',
  components: { Register },
  data() {
    return {
      fields: [
        {
          key: 'email',
          label: 'Email',
          type: 'email',
          placeholder: 'tu@email.com',
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
          placeholder: 'Tu nombre',
          icon: 'bi bi-person-circle me-1',
          required: true,
        },
        {
          key: 'roleId',
          label: 'Rol',
          type: 'select',
          placeholder: 'Selecciona un rol',
          icon: 'bi bi-person-gear me-1',
          required: true,
          options: [
            { value: 4, label: 'Cliente' },
            { value: 3, label: 'Proveedor' },
          ],
        },
      ],
    };
  },
  methods: {
    async handleRegister(form) {
      return await api.register({
        email: form.email,
        password: form.password,
        name: form.name,
        roleId: parseInt(form.roleId),
      });
    },
  },
  emits: ['cancel'],
};
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8f9fa;
  min-width: 125vh;
}

.card {
  border-radius: 0.5rem;
  background: linear-gradient(to bottom, rgba(255, 255, 255, 0.95), rgba(255, 255, 255, 0.8));
}

.form-control,
.form-select {
  border-radius: 0.375rem;
  font-size: 0.9rem;
}

.btn {
  border-radius: 0.375rem;
  font-size: 0.9rem;
}

.alert-sm {
  font-size: 0.9rem;
  padding: 0.75rem;
  margin-bottom: 0;
  display: flex;
  align-items: center;
}

label {
  font-size: 0.9rem;
}
</style>