import { authService } from "../../services/authService";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(6),
});

type FormData = z.infer<typeof schema>;

function LoginPage() {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      email: "admin@admin.com",
      password: "Admin@123",
    },
  });

  const onSubmit = async (data: FormData) => {
    try {
      await authService.login(data);
      navigate("/");
    } catch {
      alert("Invalid credentials");
    }
  };

  return (
    <div className="max-w-sm mx-auto p-6">
      <h1 className="text-2xl font-bold mb-4">Login</h1>
      <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4">
        <div>
          <label>Email</label>
          <input {...register("email")} className="input" />
          {errors.email && <p className="text-red-500">Invalid email</p>}
        </div>

        <div>
          <label>Password</label>
          <input type="password" {...register("password")} className="input" />
          {errors.password && <p className="text-red-500">Required</p>}
        </div>

        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Login
        </button>
      </form>
    </div>
  );
}

export default LoginPage;
