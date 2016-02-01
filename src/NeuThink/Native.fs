namespace NeuThink
module Native =
 open System
 open System.Runtime.InteropServices
 open Microsoft.FSharp.NativeInterop 

 let buffer = Array.init 500 (fun _ -> 0.0) 

 [<System.Runtime.InteropServices.DllImport(@"native_func.dll",EntryPoint="vectmat")>]
 extern void vectmat(double *, double *, double *, int, int, int)  

 [<System.Runtime.InteropServices.DllImport(@"libacml_mp_dll.dll",EntryPoint="dgemv")>]
 extern void dgemv(char, int, int, double, double *, int, double *, int, double, double *, int);


 let inline (~~) (data : GCHandle) = data.AddrOfPinnedObject()
 let inline (~~~) (data : GCHandle) = NativePtr.ofNativeInt (data.AddrOfPinnedObject())
 let inline (!~) (ptr  : GCHandle) = ptr.Free()
 let pin (data : double array)       = GCHandle.Alloc(data,GCHandleType.Pinned)

 let vect_mat (weights:float array) (proc_inputs:float array) (outputs:float array) =
   let weightsp = pin weights
   let proc_inputsp = pin proc_inputs
   let outputsp = pin outputs
   vectmat (NativePtr.ofNativeInt (~~proc_inputsp),NativePtr.ofNativeInt (~~weightsp),NativePtr.ofNativeInt (~~outputsp),proc_inputs.Length,weights.Length,outputs.Length)
   !~proc_inputsp
   !~weightsp
   !~outputsp   
   
 let vect_mat_dgemv  (weights:float array) (proc_inputs:float array) (outputs:float array) =
   let weightsp = pin weights
   let proc_inputsp = pin proc_inputs
   let outputsp = pin outputs
   
   dgemv('T',proc_inputs.Length,outputs.Length,1.0,~~~weightsp,proc_inputs.Length,~~~proc_inputsp,1,0.0,~~~outputsp,1)

   !~proc_inputsp
   !~weightsp
   !~outputsp   
   
