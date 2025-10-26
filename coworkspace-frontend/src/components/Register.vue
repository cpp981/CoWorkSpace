<template>
  <div class="login-container position-relative">
    <div class="position-absolute top-0 start-0 m-3">
      <img src="../assets/login_logo.png" alt="Logo" class="login-logo" />
    </div>

    <div
      class="container d-flex align-items-center justify-content-center min-vh-100"
    >
      <div class="row justify-content-center w-100">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card bg-secondary bg-opacity-25 shadow border-0">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">Crear Nueva Cuenta</h3>

              <form @submit.prevent="onSubmit" novalidate>
                <!-- Nombre -->
                <div class="mb-3">
                  <label for="name" class="form-label">Nombre</label>
                  <div class="position-relative input-icon-wrapper">
                    <input
                      id="name"
                      type="text"
                      class="form-control"
                      v-model="form.name"
                      placeholder="Tu nombre"
                      required
                      :disabled="loading"
                    />
                    <i class="bi bi-person input-icon"></i>
                  </div>
                </div>

                <!-- Email -->
                <div class="mb-3">
                  <label for="email" class="form-label">Email</label>
                  <div class="position-relative input-icon-wrapper">
                    <input
                      id="email"
                      type="email"
                      class="form-control"
                      v-model="form.email"
                      placeholder="tu@email.com"
                      required
                      :disabled="loading"
                    />
                    <i class="bi bi-envelope input-icon"></i>
                  </div>
                </div>

                <!-- Password -->
                <div class="mb-3">
                  <label for="password" class="form-label">Contraseña</label>
                  <div class="position-relative input-icon-wrapper">
                    <input
                      id="password"
                      type="password"
                      class="form-control"
                      v-model="form.password"
                      placeholder="••••••••"
                      required
                      :disabled="loading"
                    />
                    <i class="bi bi-lock input-icon"></i>
                  </div>
                </div>

                <!-- Confirmar Password -->
                <div class="mb-3">
                  <label for="passwordConfirm" class="form-label"
                    >Repetir contraseña</label
                  >
                  <div class="position-relative input-icon-wrapper">
                    <input
                      id="passwordConfirm"
                      type="password"
                      class="form-control"
                      v-model="form.passwordConfirm"
                      placeholder="••••••••"
                      required
                      :disabled="loading"
                    />
                    <i class="bi bi-lock-fill input-icon"></i>
                  </div>
                </div>

                <!-- Selector visual de rol -->
                <div class="mb-4 text-center">
                  <label
                    class="form-label d-block fw-semibold mb-2 text-primary"
                    >Selecciona tu tipo de cuenta</label
                  >
                  <div class="d-flex justify-content-center gap-3 flex-wrap">
                    <div
                      v-for="option in roleOptions"
                      :key="option.value"
                      class="role-card p-3 rounded text-center shadow-sm"
                      :class="{
                        'border-primary bg-primary bg-opacity-10':
                          form.roleId === option.value,
                        'border-secondary': form.roleId !== option.value,
                      }"
                      style="cursor: pointer; width: 140px"
                      @click="form.roleId = option.value"
                      role="button"
                      :aria-pressed="form.roleId === option.value"
                    >
                      <i :class="option.icon + ' fs-3 mb-2 d-block'"></i>
                      <div class="fw-semibold">{{ option.label }}</div>
                      <div class="small text-muted">{{ option.desc }}</div>
                    </div>
                  </div>
                </div>

                <!-- Enlace a login -->
                <div class="text-center mb-3">
                  <small class="text-muted">
                    ¿Ya tienes cuenta?
                    <a
                      href="#"
                      class="ms-1 text-primary"
                      @click.prevent="$emit('open-login')"
                      ><u>Inicia sesión</u></a
                    >
                  </small>
                </div>

                <!-- Botones -->
                <div class="d-flex justify-content-between">
                  <button
                    type="button"
                    class="btn btn-outline-secondary"
                    @click="$emit('cancel')"
                    :disabled="loading"
                  >
                    <i class="bi bi-x-circle me-2"></i> Cancelar
                  </button>

                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="loading"
                  >
                    <i class="bi bi-person-check-fill me-2"></i>
                    <span
                      v-if="loading"
                      class="spinner-border spinner-border-sm me-2"
                      role="status"
                      aria-hidden="true"
                    ></span>
                    {{ loading ? "Registrando..." : submitText }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from "vue";
import { useNotyf } from "../composables/useNotyf";
import api from "../services/api";

const notyf = useNotyf();

const props = defineProps({
  submitButtonText: { type: String, default: "Crear cuenta" },
});
const emit = defineEmits(["cancel", "open-login", "registered"]);

const loading = ref(false);
const submitText = props.submitButtonText;

const form = reactive({
  name: "",
  email: "",
  password: "",
  passwordConfirm: "",
  roleId: null,
});

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

const onSubmit = async () => {
  // Validaciones
  if (!form.name || !form.email || !form.password || !form.passwordConfirm) {
    notyf.error("Completa todos los campos obligatorios.");
    return;
  }
  if (form.password !== form.passwordConfirm) {
    notyf.error("Las contraseñas no coinciden.");
    return;
  }
  if (!form.roleId) {
    notyf.error("Por favor selecciona el tipo de cuenta.");
    return;
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
    const msg = response?.data?.message || "Registro exitoso";
    notyf.success(msg);

    // emitimos al padre para redirección
    emit("registered", { success: true, data: response?.data || null });

    // limpiamos formulario
    form.name = "";
    form.email = "";
    form.password = "";
    form.passwordConfirm = "";
    form.roleId = null;
  } catch (err) {
    const errorMsg =
      err?.response?.data?.message || err?.message || "Error al registrar";
    notyf.error(errorMsg);
  } finally {
    loading.value = false;
  }
};
</script>
