import { useEffect, useState } from "react";

import { Employee } from "../../types/employee/Employee";
import { departmentService } from "../../services/departmentService";
import { employeeService } from "../../services/employeeService";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { useParams } from "react-router-dom";

function EmployeeDetailsPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [departmentId, setDepartmentId] = useState<string>("");
  const [departments, setDepartments] = useState<
    { id: string; name: string }[]
  >([]);

  useEffect(() => {
    const fetchData = async () => {
      const emp = await employeeService.getById(id!);
      const depts = await departmentService.getAll();
      setEmployee(emp);
      setDepartments(depts);
      const currentDept = depts.find((d) => d.name === emp.departmentName);
      if (currentDept) setDepartmentId(currentDept.id);
    };

    fetchData();
  }, [id]);

  const handleUpdate = async () => {
    if (!employee || !departmentId) return;

    try {
      await employeeService.update(employee.id, {
        ...employee,
        departmentId,
      });

      toast.success("Department updated successfully!");

      setTimeout(() => {
        navigate("/");
      }, 500);
    } catch (error) {
      toast.error("Error updating department. Please try again.");
    }
  };
  if (!employee) return <p>Loading...</p>;

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Employee Details</h1>

      <div className="space-y-2">
        <p>
          <strong>Name:</strong> {employee.firstName} {employee.lastName}
        </p>
        <p>
          <strong>Hire Date:</strong>{" "}
          {new Date(employee.hireDate).toLocaleDateString()}
        </p>
        <p>
          <strong>Phone:</strong> {employee.phone || "-"}
        </p>
        <p>
          <strong>Address:</strong> {employee.address || "-"}
        </p>

        <div>
          <label>
            <strong>Department</strong>
          </label>
          <select
            className="input"
            value={departmentId}
            onChange={(e) => setDepartmentId(e.target.value)}
          >
            {departments.map((dept) => (
              <option key={dept.id} value={dept.id}>
                {dept.name}
              </option>
            ))}
          </select>
        </div>

        <button
          onClick={handleUpdate}
          className="bg-blue-600 text-white px-4 py-2 mt-4 rounded hover:bg-blue-700"
        >
          Update
        </button>
      </div>
    </div>
  );
}

export default EmployeeDetailsPage;
