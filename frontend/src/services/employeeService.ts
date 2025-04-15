import {
  CreateEmployeeDto,
  Employee,
  EmployeePagedQuery,
  PaginatedResult,
  UpdateEmployeeDto,
} from "../types/employee/Employee";

import { api } from "./api";

export const employeeService = {
  async getAll(): Promise<Employee[]> {
    const response = await api.get("/employee");
    return response.data;
  },

  async getPaged(
    query: EmployeePagedQuery
  ): Promise<PaginatedResult<Employee>> {
    const response = await api.post("/employee/paged", query);
    return response.data;
  },

  async getById(id: string): Promise<Employee> {
    const response = await api.get(`/employee/${id}`);
    return response.data;
  },

  async create(data: CreateEmployeeDto): Promise<void> {
    await api.post("/employee", data);
  },

  async update(id: string, data: UpdateEmployeeDto): Promise<void> {
    await api.put(`/employee/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/employee/${id}`);
  },
};
