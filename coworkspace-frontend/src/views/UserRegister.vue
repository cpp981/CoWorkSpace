<template>
  <div class="login-container position-relative">
    <div class="position-absolute top-0 start-0 m-3">
      <img src="../assets/login_logo.png" alt="Logo" class="login-logo" />
    </div>

    <div
      class="container d-flex align-items-center justify-content-center min-vh-100"
    >
      <Register
        :title="'Crear Nueva Cuenta'"
        :fields="fields"
        :submitButtonText="submitText"
        :submitHandler="handleSubmit"
        link-text="¿Ya tienes cuenta?"
        link-label="Inicia sesión"
        @cancel="$emit('cancel')"
      />
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import Register from "../components/Register.vue";
import { useNotyf } from "../composables/useNotyf";
import api from "../services/api";

const notyf = useNotyf();

const props = defineProps({
  submitButtonText: { type: String, default: "Crear cuenta" },
});
const emit = defineEmits(["cancel", "open-login", "registered"]);

const submitText = props.submitButtonText;
const loading = ref(false);

// Opciones de rol
const roleOptions = [
  {
    value: 4,
    label: "Cliente",
    icon: "bi bi-person",
    desc: "Reserva espacios",
  },
  {
    value: 3,
    label: "Proveedor",
    icon: "bi bi-building",
    desc: "Publica tus espacios",
  },
];

// Campos del formulario
const fields = [
  {
    key: "name",
    label: "Nombre",
    type: "text",
    placeholder: "Tu nombre",
    icon: "bi bi-person",
    required: true,
  },
  {
    key: "email",
    label: "Email",
    type: "email",
    placeholder: "tu@email.com",
    icon: "bi bi-envelope",
    required: true,
  },
  {
    key: "password",
    label: "Contraseña",
    type: "password",
    placeholder: "••••••••",
    icon: "bi bi-lock",
    required: true,
  },
  {
    key: "passwordConfirm",
    label: "Repetir contraseña",
    type: "password",
    placeholder: "••••••••",
    icon: "bi bi-lock-fill",
    required: true,
  },
  {
    key: "roleId",
    label: "Selecciona tu tipo de cuenta",
    type: "roleSelect",
    options: roleOptions,
    required: true,
  },
];

const handleSubmit = async (form) => {
  // Validaciones
  if (!form.name || !form.email || !form.password || !form.passwordConfirm) {
    notyf.error("Completa todos los campos obligatorios.");
    throw new Error("Completa todos los campos obligatorios.");
  }
  if (form.password !== form.passwordConfirm) {
    notyf.error("Las contraseñas no coinciden.");
    throw new Error("Las contraseñas no coinciden.");
  }
  if (!form.roleId) {
    notyf.error("Por favor selecciona el tipo de cuenta.");
    throw new Error("Por favor selecciona el tipo de cuenta.");
  }

  loading.value = true;
  try {
    const payload = {
      name: form.name,
      email: form.email,
      password: form.password,
      roleId: form.roleId,
    };
    const response = await api.register(payload);
    notyf.success(response?.data?.message || "Registro exitoso");
    emit("registered", { success: true, data: response?.data || null });
    return response;
  } catch (err) {
    notyf.error(
      err?.response?.data?.message || err?.message || "Error al registrar"
    );
    throw err;
  } finally {
    loading.value = false;
  }
};
</script>
