import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = 'http://localhost:5257';
const PRODUCT_ID = 'c3d4e5f6-a7b8-9012-cdef-123456789012';

const N = parseInt(__ENV.N) || 2;
const M = parseInt(__ENV.M) || 4;
const TARGET = parseInt(__ENV.TARGET) || 5;
const DURATION = __ENV.DURATION || '2m';

export const options = {
  stages: [
    { duration: '30s', target: TARGET },
    { duration: DURATION, target: TARGET },
    { duration: '30s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<5000'],
    http_req_failed: ['rate<0.05'],
  },
};

export default function () {
  for (let i = 0; i < N; i++) {
    const nombre = `K6-VU${__VU}-C${i}-${Date.now()}`;

    const resCliente = http.post(`${BASE_URL}/api/clientes`,
      JSON.stringify(nombre),
      { headers: { 'Content-Type': 'application/json' }, tags: { endpoint: 'crear-cliente' } }
    );
    check(resCliente, {
      'cliente creado': (r) => r.status === 200,
    });
    if (resCliente.status !== 200) {
      sleep(1);
      continue;
    }
    const cliente = resCliente.json();

    for (let j = 0; j < M; j++) {
      const payload = JSON.stringify({
        cliente: { id: cliente.id },
        detalles: [
          {
            producto: { id: PRODUCT_ID },
            cantidad: (j % 3) + 1,
          },
        ],
      });

      const resPedido = http.post(`${BASE_URL}/api/pedidos`,
        payload,
        { headers: { 'Content-Type': 'application/json' }, tags: { endpoint: 'crear-pedido' } }
      );
      check(resPedido, {
        'pedido creado': (r) => r.status === 200,
      });
      if (resPedido.status !== 200) {
        sleep(1);
        continue;
      }
      const pedidoId = JSON.parse(resPedido.body);

      if (j % 2 === 0) {
        const resConfirm = http.put(`${BASE_URL}/api/pedidos/${pedidoId}/confirmar`,
          null,
          { tags: { endpoint: 'confirmar-pedido' } }
        );
        check(resConfirm, {
          'pedido confirmado': (r) => r.status === 200,
        });
      } else {
        const resCancel = http.put(`${BASE_URL}/api/pedidos/${pedidoId}/cancelar`,
          null,
          { tags: { endpoint: 'cancelar-pedido' } }
        );
        check(resCancel, {
          'pedido cancelado': (r) => r.status === 200,
        });
      }
    }
  }

  sleep(1);
}
