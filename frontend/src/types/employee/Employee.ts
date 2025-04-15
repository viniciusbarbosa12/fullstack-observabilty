export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  hireDate: string;
  phone?: string;
  address?: string;
  departmentName: string;
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  hireDate: string;
  phone?: string;
  address?: string;
  departmentId: string;
}

export interface UpdateEmployeeDto extends CreateEmployeeDto {
  id: string;
}

export interface EmployeePagedQuery {
  pageSize: number;
  page: number;
  filter?: {
    name?: string;
    departmentId?: string;
  };
}

export interface PaginatedResult<T> {
  totalCount: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  items: T[];
}
