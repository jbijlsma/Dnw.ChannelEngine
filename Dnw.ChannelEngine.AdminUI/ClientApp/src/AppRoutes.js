import { Counter } from "./components/Counter";
import { MerchantChannelSimulation } from "./components/MerchantChannelSimulation";

const AppRoutes = [
  {
    index: true,
    element: <MerchantChannelSimulation />
  },
  {
    path: '/counter',
    element: <Counter />
  }
];

export default AppRoutes;
