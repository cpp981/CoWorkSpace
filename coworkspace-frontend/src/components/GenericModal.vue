<template>
    <div class="modal fade" tabindex="-1" role="dialog" ref="modalEl" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <form @submit.prevent="handleSubmit">
                    <div class="modal-header">
                        <h3 class="modal-title text-primary">{{ title }}</h3>
                        <button type="button" class="btn-close" @click="hide"></button>
                    </div>

                    <div class="modal-body">
                        <!-- aquí van los campos que el padre inyecta -->
                        <slot />
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="hide">
                            Cancelar
                        </button>
                        <button type="submit" class="btn btn-primary">
                            {{ confirmText }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";
import { Modal } from "bootstrap";

const props = defineProps({
    title: { type: String, required: true },
    confirmText: { type: String, default: "Guardar" },
    modelValue: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "submit"]);

const modalEl = ref(null);
let modalInstance = null;

onMounted(() => {
    if (modalEl.value) {
        modalInstance = new Modal(modalEl.value, { backdrop: "static" });
    }
});

onBeforeUnmount(() => {
    modalInstance = null;
});

watch(
    () => props.modelValue,
    (value) => {
        if (!modalInstance) return;
        value ? modalInstance.show() : modalInstance.hide();
    }
);

const hide = () => {
    if (modalInstance) modalInstance.hide();
    emit("update:modelValue", false);
};

const handleSubmit = () => {
    emit("submit");
};
</script>