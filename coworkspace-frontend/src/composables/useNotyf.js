import { Notyf } from 'notyf';
import 'notyf/notyf.min.css';

let notyfInstance;

export function useNotyf() {
  if (!notyfInstance) {
    notyfInstance = new Notyf({
      duration: 4000,
      ripple: true,
      position: {
        x: 'right',
        y: 'top',
      },
    });
  }

  return notyfInstance;
}
