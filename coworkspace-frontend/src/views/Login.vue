<template>
  <div class="login-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card shadow border-0 bg-secondary bg-opacity-25">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">Iniciar Sesión</h3>
              <!-- Mensaje visible dentro de la card -->
              <div v-if="msg" class="alert alert-danger py-2 px-3">
                {{ msg }}
              </div>
              <form @submit.prevent="handleLogin">
                <div class="mb-3 input-icon-wrapper">
                  <input
                    v-model="form.email"
                    type="email"
                    class="form-control"
                    id="email"
                    placeholder="tu@email.com"
                    required
                    :disabled="loading"
                  />
                  <i class="bi bi-person input-icon"></i>
                </div>
                <div class="mb-3 input-icon-wrapper">
                  <input
                    v-model="form.password"
                    type="password"
                    class="form-control"
                    id="password"
                    placeholder="••••••••"
                    required
                    :disabled="loading"
                  />
                  <i class="bi bi-lock input-icon"></i>
                </div>
                <div class="d-grid gap-2 mb-2">
                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="loading"
                  >
                    <i class="bi bi-box-arrow-in-right me-2"></i>
                    {{ loading ? "Iniciando..." : "Iniciar Sesión" }}
                  </button>
                  <button
                    type="button"
                    class="btn btn-outline-secondary"
                    @click="$emit('cancel')"
                    :disabled="loading"
                  >
                    Volver
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

<script>
import { ref } from "vue";
import { useAuthStore } from "../stores/auth";
import { useNotyf } from "../composables/useNotyf";

export default {
  name: "LoginPage",
  emits: ["cancel", "login-success"],
  setup(_, { emit }) {
    const form = ref({
      email: "",
      password: "",
    });
    const loading = ref(false);
    const msg = ref("");

    const authStore = useAuthStore();
    const notyf = useNotyf();

    const handleLogin = async () => {
      loading.value = true;
      msg.value = "";
      try {
        const message = await authStore.login({
          email: form.value.email,
          password: form.value.password,
        });
        notyf.success(message || "Inicio de sesión exitoso");
        msg.value = message || "Inicio de sesión exitoso";
        form.value.email = "";
        form.value.password = "";
        emit("login-success");
      } catch (error) {
        const errorMsg =
          error?.response?.data?.message ||
          (typeof error === "string" ? error : error?.message) ||
          "Error al iniciar sesión";
        notyf.error(errorMsg);
      } finally {
        loading.value = false;
      }
    };

    return {
      form,
      loading,
      handleLogin,
      msg,
    };
  },
};
</script>
