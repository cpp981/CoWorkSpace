<template>
  <div class="register-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card shadow border-0">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">{{ title }}</h3>
              <form @submit.prevent="handleSubmit" class="">
                <div v-for="field in fields" :key="field.key" class="mb-2">
                  <label :for="field.key" class="form-label">
                    <i :class="field.icon"></i>{{ field.label }}
                  </label>
                  <input
                    v-if="field.type !== 'select'"
                    v-model="form[field.key]"
                    :type="field.type"
                    class="form-control border border-dark"
                    :id="field.key"
                    :placeholder="field.placeholder"
                    :required="field.required"
                    @input="field.key === 'password' || field.key === 'confirmPassword' ? updatePasswordStrength(field.key) : null"
                  />
                  <select
                    v-else
                    v-model="form[field.key]"
                    class="form-select border border-dark"
                    :id="field.key"
                    :required="field.required"
                  >
                    <option value="" disabled>{{ field.placeholder }}</option>
                    <option v-for="option in field.options" :key="option.value" :value="option.value">
                      {{ option.label }}
                    </option>
                  </select>
                  <div v-if="field.key === 'password' && passwordStrength" class="form-text">
                    Fortaleza: <span :class="passwordStrengthClass">{{ passwordStrength }}</span>
                    <div class="progress mt-1" style="height: 7px;">
                      <div class="progress-bar"
                        :class="passwordProgressClass"
                        role="progress"
                        :style="{ width: passwordProgressWidth }"
                        aria-valuenow="7" 
                        aria-valuemin="0"
                        aria-valuemax="100"
                      ></div>
                    </div>
                  </div>
                  <div v-if="field.key === 'confirmPassword'">
                    <div v-if="confirmPasswordStrength" class="form-text">
                      Fortaleza: <span :class="confirmPasswordStrengthClass">{{ confirmPasswordStrength }}</span>
                      <div class="progress mt-1" style="height: 7px;">
                        <div class="progress-bar"
                          :class="confirmPasswordProgressClass"
                          role="progressbar"
                          :style="{ width: confirmPasswordProgressWidth }"
                          aria-valuenow="7"
                          aria-valuemin="0"
                          aria-valuemax="100"
                        ></div>
                      </div>
                    </div>
                    <div v-if="confirmPasswordError" class="text-danger small">
                      Las contraseñas no coinciden
                    </div>
                  </div>
                </div>
                <div class="d-grid gap-2 mb-2">
                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="loading || confirmPasswordError"
                  >
                    <i class="button bi bi-person-plus me-2"></i>
                    {{ loading ? 'Registrando...' : submitButtonText }}
                  </button>
                  <button
                    type="button"
                    class="button btn btn-outline-secondary"
                    @click="$emit('cancel')"
                  >
                    Volver
                  </button>
                </div>
                <div v-if="error" class="alert alert-danger alert-sm" role="alert">
                  <i class="bi bi-exclamation-circle me-2"></i>{{ error }}</div>
                <div v-if="success" class="alert alert-success alert-sm" role="alert">
                  <i class="bi bi-check-circle me-2"></i>{{ success }}
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
export default {
  name: 'Register',
  props: {
    title: {
      type: String,
      default: 'Registrarse',
    },
    fields: {
      type: Array,
      required: true,
    },
    submitButtonText: {
      type: String,
      default: 'Registrarse',
    },
    submitHandler: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      form: {},
      loading: false,
      error: null,
      success: null,
      passwordStrength: null,
      passwordStrengthClass: null,
      passwordProgressClass: null,
      passwordProgressWidth: '0%',
      confirmPasswordStrength: null,
      confirmPasswordStrengthClass: null,
      confirmPasswordProgressClass: null,
      confirmPasswordProgressWidth: '0%',
      confirmPasswordError: false,
    };
  },
  methods: {
    updatePasswordStrength(fieldKey) {
      const value = this.form[fieldKey] || '';
      let strength, strengthClass, progressClass, progressWidth;

      if (value.length >= 12 && /[a-zA-Z]/.test(value) && /\d/.test(value) && /[^a-zA-Z\d]/.test(value)) {
        strength = 'Fuerte';
        strengthClass = 'text-success';
        progressClass = 'bg-success';
        progressWidth = '100%';
      } else if (value.length >= 8 && (/[a-zA-Z]/.test(value) + /\d/.test(value) + /[^a-zA-Z\d]/.test(value) >= 2)) {
        strength = 'Media';
        strengthClass = 'text-warning';
        progressClass = 'bg-warning';
        progressWidth = '66%';
      } else if (value.length > 0) {
        strength = 'Débil';
        strengthClass = 'text-danger';
        progressClass = 'bg-danger';
        progressWidth = '33%';
      } else {
        strength = null;
        strengthClass = null;
        progressClass = null;
        progressWidth = '0%';
      }

      if (fieldKey === 'password') {
        this.passwordStrength = strength;
        this.passwordStrengthClass = strengthClass;
        this.passwordProgressClass = progressClass;
        this.passwordProgressWidth = progressWidth;
      } else if (fieldKey === 'confirmPassword') {
        this.confirmPasswordStrength = strength;
        this.confirmPasswordStrengthClass = strengthClass;
        this.confirmPasswordProgressClass = progressClass;
        this.confirmPasswordProgressWidth = progressWidth;
      }

      // Validar Repite la contraseña
      this.confirmPasswordError = this.form.confirmPassword && this.form.password !== this.form.confirmPassword;
    },
    async handleSubmit() {
      if (this.confirmPasswordError) {
        this.error = 'Las contraseñas no coinciden';
        return;
      }

      this.loading = true;
      this.error = null;
      this.success = null;

      try {
        const response = await this.submitHandler(this.form);
        this.success = response.data.message;
        this.form = {};
        this.passwordStrength = null;
        this.passwordStrengthClass = null;
        this.passwordProgressClass = null;
        this.passwordProgressWidth = '0%';
        this.confirmPasswordStrength = null;
        this.confirmPasswordStrengthClass = null;
        this.confirmPasswordProgressClass = null;
        this.confirmPasswordProgressWidth = '0%';
        this.confirmPasswordError = false;
      } catch (error) {
        this.error = error.response?.data?.message || 'Error al registrar';
      } finally {
        this.loading = false;
      }
    },
  },
  emits: ['cancel'],
};
</script>

<style scoped>
.register-container {
  padding-top: 2rem;
  padding-bottom: 2rem;
}
.progress {
  border-radius: 4px;
}
</style>