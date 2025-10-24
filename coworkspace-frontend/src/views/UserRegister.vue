<template>
  <Register
    title="Registrarse"
    :fields="fields"
    submit-button-text="Registrarse"
    :submit-handler="handleRegister"
    @cancel="$emit('cancel')"
  />
</template>

<script setup>
import { ref } from "vue";
import Register from "../components/Register.vue";
import api from "../services/api";

defineEmits(["cancel"]);

const fields = ref([
  {
    key: "email",
    label: "Email",
    type: "email",
    placeholder: "tu@email.com",
    icon: "bi bi-envelope-at me-1",
    required: true,
  },
  {
    key: "password",
    label: "Contraseña",
    type: "password",
    placeholder: "••••••••",
    icon: "bi bi-lock me-1",
    required: true,
  },
  {
    key: "confirmPassword",
    label: "Repite la contraseña",
    type: "password",
    placeholder: "••••••••",
    icon: "bi bi-lock me-1",
    required: true,
  },
  {
    key: "name",
    label: "Nombre",
    type: "text",
    placeholder: "Tu nombre",
    icon: "bi bi-person me-1",
    required: true,
  },
  {
    key: "roleId",
    label: "Rol",
    type: "select",
    placeholder: "Selecciona un rol",
    icon: "bi bi-person-gear me-1",
    required: true,
    options: [
      { value: 4, label: "Cliente" },
      { value: 3, label: "Proveedor" },
    ],
  },
]);

const handleRegister = async (form) => {
  return await api.register({
    email: form.email,
    password: form.password,
    name: form.name,
    roleId: parseInt(form.roleId),
  });
};
</script>
