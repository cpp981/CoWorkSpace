<template>
  <div class="container reg-cont">
    <div class="row justify-content-center">
      <div class="col-11 col-md-8 col-lg-5">
        <div class="card bg-secondary bg-opacity-25 shadow border-0">
          <div class="card-body p-3">
            <form @submit.prevent="onSubmit">
              <h2 class="mb-4 fw-bold">{{ title }}</h2>

              <div v-for="field in fields" :key="field.key" class="mb-3">
                <!-- Inputs normales con icono a la derecha -->
                <div
                  v-if="field.type !== 'roleSelect' && field.type !== 'select'"
                  class="input-group"
                >
                  <input
                    :type="field.type"
                    class="form-control"
                    :id="field.key"
                    v-model="form[field.key]"
                    :placeholder="field.placeholder"
                    :required="field.required"
                    :disabled="loading"
                  />
                  <span class="input-group-text">
                    <i :class="field.icon"></i>
                  </span>
                </div>

                <!-- Select tradicional -->
                <select
                  v-else-if="field.type === 'select'"
                  class="form-select"
                  :id="field.key"
                  v-model="form[field.key]"
                  :required="field.required"
                  :disabled="loading"
                >
                  <option disabled value="">{{ field.placeholder }}</option>
                  <option
                    v-for="option in field.options"
                    :key="option.value"
                    :value="option.value"
                  >
                    {{ option.label }}
                  </option>
                </select>

                <!-- Selector (cards) visual de rol  -->
                <div v-else-if="field.type === 'roleSelect'" class="mb-3">
                  <label
                    class="form-label d-block fw-semibold mb-2 text-primary text-center"
                  >
                    {{ field.label }}
                  </label>
                  <div class="d-flex justify-content-center gap-3 flex-wrap">
                    <div
                      v-for="option in field.options"
                      :key="option.value"
                      class="role-card p-3 rounded text-center shadow-sm"
                      :class="{
                        'border-primary bg-primary bg-opacity-10':
                          form[field.key] === option.value,
                        'border-secondary': form[field.key] !== option.value,
                      }"
                      style="cursor: pointer; width: 140px"
                      @click="form[field.key] = option.value"
                      role="button"
                      :aria-pressed="form[field.key] === option.value"
                    >
                      <i :class="option.icon + ' fs-3 mb-2 d-block'"></i>
                      <div class="fw-semibold">{{ option.label }}</div>
                      <div class="small text-muted">{{ option.desc }}</div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Frase con enlace opcional -->
              <div v-if="linkText && linkLabel" class="text-center mb-3">
                <small class="text-muted">
                  {{ linkText }}
                  <a
                    href="#"
                    class="ms-1 text-primary"
                    @click.prevent="goToLogin"
                  >
                    <u>{{ linkLabel }}</u>
                  </a>
                </small>
              </div>

              <!-- Botones -->
              <div class="d-flex justify-content-between">
                <button
                  type="button"
                  class="btn btn-outline-secondary"
                  @click="emitCancel"
                  :disabled="loading"
                >
                  <i class="bi bi-x-circle me-2"></i>
                  Cancelar
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
                  {{ loading ? "Registrando..." : submitButtonText }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from "vue";

const props = defineProps({
  title: { type: String, default: "" },
  fields: { type: Array, default: () => [] },
  submitButtonText: { type: String, default: "Enviar" },
  submitHandler: { type: Function, default: null },
  linkText: { type: String, default: "" },
  linkLabel: { type: String, default: "" },
});

const emit = defineEmits(["cancel", "submitted", "error", "go-login"]);

const form = reactive({});
const loading = ref(false);

const initializeForm = () => {
  props.fields.forEach((field) => {
    form[field.key] = field?.value ?? "";
  });
};

onMounted(() => initializeForm());

const emitCancel = () => emit("cancel");

const onSubmit = async () => {
  loading.value = true;
  try {
    let response = null;
    if (typeof props.submitHandler === "function") {
      response = await props.submitHandler(form);
    } else {
      emit("submitted", form);
    }
    const msg = response?.data?.message || "Registro exitoso";
    if (response) initializeForm();
  } catch (error) {
    const errorMsg =
      error?.response?.data?.message || error?.message || "Error al registrar";
    emit("error", error);
  } finally {
    loading.value = false;
  }
};

const goToLogin = () => {
  emit("go-login");
};
</script>
