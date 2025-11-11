using ClientesPedidos.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientesPedidos.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (ServiceReference1.ServicePedidosClient WCF = new ServiceReference1.ServicePedidosClient())
            using (ServiceReference2.ServiceClientesClient WCFClientes = new ServiceReference2.ServiceClientesClient())
            using (ServiceReference3.ServiceProducClient WCFProduc = new ServiceReference3.ServiceProducClient())
            {
                var pedidos = WCF.Obtener().ToList();
                var clientes = WCFClientes.Obtener().ToList();
                var productos = WCFProduc.Obtener().ToList();


                ViewBag.Clientes = clientes.ToDictionary(c => c.IdC, c => c.Nombre);
                ViewBag.Productos = productos.ToDictionary(p => p.idPro, p => p.Nombre);

                return View(pedidos);
            }
        }



        public ActionResult Agregar()
        {
            using (ServiceReference2.ServiceClientesClient WCFClientes = new ServiceReference2.ServiceClientesClient())
            using (ServiceReference3.ServiceProducClient WCFProduc= new ServiceReference3.ServiceProducClient())
            {
                var clientes = WCFClientes.Obtener().ToList();
                var productos = WCFProduc.Obtener().ToList();


                ViewBag.Clientes = new SelectList(clientes, "IdC", "Nombre");
                ViewBag.Productos = new SelectList(productos, "IdPro", "Nombre");
            }

            return View();
        }


        public ActionResult AgregarPost(PedidosEx model)
        {

            if (model.ProductosId == null || model.ProductosId == 0)
            {
                ModelState.AddModelError("ProductosId", "Debe seleccionar un producto.");
            }
            if (ModelState.IsValid)
            {
                using (ServiceReference1.ServicePedidosClient WCF = new ServiceReference1.ServicePedidosClient())
                {
                    WCF.Agregar(model);
                }
                return RedirectToAction("Index");
            }

            
            using (ServiceReference2.ServiceClientesClient WCFClientes = new ServiceReference2.ServiceClientesClient())
            {
                var clientes = WCFClientes.Obtener().ToList();
                ViewBag.Clientes = new SelectList(clientes, "IdC", "Nombre");
            }
            using (ServiceReference3.ServiceProducClient WCFProduc= new ServiceReference3.ServiceProducClient())
            {
                var productos = WCFProduc.Obtener().ToList();
                ViewBag.Productos = new SelectList(productos, "IdPro", "Nombre");
            }

            return View(model);
        }


        public ActionResult Editar(int id)
        {
            using (ServiceReference1.ServicePedidosClient WCF = new ServiceReference1.ServicePedidosClient())
            using (ServiceReference2.ServiceClientesClient WCFClientes = new ServiceReference2.ServiceClientesClient())
            using (ServiceReference3.ServiceProducClient WCFProduc = new ServiceReference3.ServiceProducClient())
            {
                var pedido = WCF.BuscarPorId(id);
                var clientes = WCFClientes.Obtener().ToList();
                var productos = WCFProduc.Obtener().ToList();

                ViewBag.Clientes = new SelectList(clientes, "IdC", "Nombre", pedido.ClientesId);
                ViewBag.Produc = new SelectList(productos, "IdPro", "Nombre", pedido.ProductosId);

                return View(pedido);
            }
        }
            

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPost(PedidosEx pedido)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (ServiceReference1.ServicePedidosClient WCF = new ServiceReference1.ServicePedidosClient())
                    {
                        WCF.Editar(pedido);
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al editar pedido: " + ex.Message;
            }
            using (ServiceReference2.ServiceClientesClient WCFClientes = new ServiceReference2.ServiceClientesClient())
            using (ServiceReference3.ServiceProducClient WCFProduc = new ServiceReference3.ServiceProducClient())
            {
                var clientes = WCFClientes.Obtener().ToList();
                ViewBag.Clientes = new SelectList(clientes, "IdC", "Nombre", pedido.ClientesId);
                var productos = WCFProduc.Obtener().ToList();
                ViewBag.Produc = new SelectList(productos, "IdPro", "Nombre", pedido.ProductosId);
            }

            return View("Editar", pedido);
        }

        public ActionResult Eliminar(int id)
        {
            using (var WCF = new ServiceReference1.ServicePedidosClient())
            using (var WCFClientes = new ServiceReference2.ServiceClientesClient())
            using (var WCFProduc = new ServiceReference3.ServiceProducClient())
            {
                try
                {
                    var pedido = WCF.BuscarPorId(id);
                    var clientes = WCFClientes.Obtener().ToList();
                    var productos = WCFProduc.Obtener().ToList();

                    var cliente = clientes.FirstOrDefault(c => c.IdC == pedido.ClientesId);
                    ViewBag.NombreCliente = cliente != null ? cliente.Nombre : "Desconocido";
                    var producto = productos.FirstOrDefault(p => p.idPro == pedido.ProductosId);
                    ViewBag.NombreProducto = producto != null ? producto.Nombre : "Desconocido";

                    return View(pedido);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al cargar el pedido: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }
        public ActionResult EliminarPost(PedidosEx pedido)
        {
            using (var WCF = new ServiceReference1.ServicePedidosClient())
            {
                try
                {
                    WCF.Eliminar(pedido);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al eliminar pedido: " + ex.Message;
                    return View("Eliminar", pedido);
                }
            }
        }




        public ActionResult Buscar(string texto)
        {
            using (ServiceReference1.ServicePedidosClient WCF = new ServiceReference1.ServicePedidosClient())
            {
                try
                {
                    List<PedidosEx> ls = WCF.BuscarPorClientes(texto).ToList();
                    return View("Index", ls);
                }
                catch (Exception ex)
                {
                    TempData["e"] = ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }




    }
}