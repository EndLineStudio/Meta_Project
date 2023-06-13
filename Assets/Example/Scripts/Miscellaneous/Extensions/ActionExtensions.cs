namespace Example
{
	using System;
	using System.Runtime.CompilerServices;
	using UnityEngine;

	public static partial class ActionExtensions
	{
		// PUBLIC METHODS

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke                                (this Action                                 action)                                                                                  { if (action != null) { try { action();                                               } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1>                            (this Action<T1>                             action, T1 arg1)                                                                         { if (action != null) { try { action(arg1);                                           } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2>                        (this Action<T1, T2>                         action, T1 arg1, T2 arg2)                                                                { if (action != null) { try { action(arg1, arg2);                                     } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3>                    (this Action<T1, T2, T3>                     action, T1 arg1, T2 arg2, T3 arg3)                                                       { if (action != null) { try { action(arg1, arg2, arg3);                               } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3, T4>                (this Action<T1, T2, T3, T4>                 action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)                                              { if (action != null) { try { action(arg1, arg2, arg3, arg4);                         } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3, T4, T5>            (this Action<T1, T2, T3, T4, T5>             action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)                                     { if (action != null) { try { action(arg1, arg2, arg3, arg4, arg5);                   } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3, T4, T5, T6>        (this Action<T1, T2, T3, T4, T5, T6>         action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)                            { if (action != null) { try { action(arg1, arg2, arg3, arg4, arg5, arg6);             } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7>    (this Action<T1, T2, T3, T4, T5, T6, T7>     action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)                   { if (action != null) { try { action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);       } catch (Exception exception) { Debug.LogException(exception); } } }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)          { if (action != null) { try { action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); } catch (Exception exception) { Debug.LogException(exception); } } }
	}
}
