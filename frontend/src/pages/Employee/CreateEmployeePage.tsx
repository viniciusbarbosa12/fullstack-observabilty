import { CreateEmployeeDto } from "../../types/employee/Employee";
import { departmentService } from "../../services/departmentService";
import { employeeService } from "../../services/employeeService";
import { toast } from "react-toastify";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const schema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  hireDate: z.string().min(1),
  phone: z.string().optional(),
  address: z.string().optional(),
  departmentId: z.string().uuid(),
});

type FormData = z.infer<typeof schema>;

function CreateEmployeePage() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const { data: departments } = useQuery({
    queryKey: ["departments"],
    queryFn: departmentService.getAll,
  });

  const onSubmit = async (data: FormData) => {
    try {
      await employeeService.create(data as CreateEmployeeDto);
      toast.success("Department created successfully!");

      setTimeout(() => {
        navigate("/");
      }, 500);
    } catch (error) {
      toast.error("Error creating department. Please try again.");
    }
  };

  return (
    <div className="max-w-xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Create New Employee</h1>
      <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
        <div>
          <label>First Name</label>
          <input {...register("firstName")} className="input" />
          {errors.firstName && <p className="text-red-500">Required</p>}
        </div>

        <div>
          <label>Last Name</label>
          <input {...register("lastName")} className="input" />
          {errors.lastName && <p className="text-red-500">Required</p>}
        </div>

        <div>
          <label>Hire Date</label>
          <input type="date" {...register("hireDate")} className="input" />
          {errors.hireDate && <p className="text-red-500">Required</p>}
        </div>

        <div>
          <label>Phone</label>
          <input {...register("phone")} className="input" />
        </div>

        <div>
          <label>Address</label>
          <input {...register("address")} className="input" />
        </div>

        <div>
          <label>Department</label>
          <select {...register("departmentId")} className="input">
            <option value="">Select...</option>
            {departments?.map((dept) => (
              <option key={dept.id} value={dept.id}>
                {dept.name}
              </option>
            ))}
          </select>
          {errors.departmentId && <p className="text-red-500">Required</p>}
        </div>

        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Save
        </button>
      </form>
    </div>
  );
}

export default CreateEmployeePage;
