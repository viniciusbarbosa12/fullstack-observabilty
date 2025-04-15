import { api } from "./api";

export interface Department {
  id: string;
  name: string;
}

export const departmentService = {
  async getAll(): Promise<Department[]> {
    const response = await api.get("/department");
    return response.data;
  },
};
