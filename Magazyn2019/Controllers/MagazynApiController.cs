using System;
using System.Linq;
using System.Web.Http;
using Magazyn2019.Models;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using Magazyn2019.UnitOfWorks;

namespace Magazyn2019.Controllers
{
    [RoutePrefix("")]
    public class MagazynApiController : ApiController
    {
        private UnitOfWork unitOfWork;
        public MagazynApiController()
        {
            unitOfWork = new UnitOfWork( new Magazyn2019Entities());
        }
        public User getActiveUser()
        {
            try
            {
                int idUser = (int)HttpContext.Current.Session["ActiveUserId"];
                User activeUser = unitOfWork.UserRepository.GetActiveUser(idUser);
                return activeUser;
            }
            catch (Exception)
            {
                return null;
            }

        }
        [HttpGet]
        [Route("Warehouses")]
        public dynamic getWarehouses()
        {
            var wareHouses = unitOfWork.WarehouseRepository.GetAllWarehouses();
            return wareHouses;
        }
        [HttpGet]
        [Route("Warehouses/{id}")]
        public dynamic getWarehouseForId(int id)
        {
            var warehouse = unitOfWork.WarehouseRepository.GetWarehouseByID(id);
            return Json(warehouse);
        }
        [HttpDelete]
        [Route("Warehouses/{id}")]
        public void deleteWarehouse(int id)
        {
            try
            {
                unitOfWork.WarehouseRepository.Delete(id);
                unitOfWork.Complete();
            }
            catch (Exception)
            {

            }
        }
        [HttpPost]
        [Route("Warehouses")]
        public int postWarehouse(JObject jsonResult)
        {

            Warehouse warehouse = new Warehouse();
            User activeUser = getActiveUser();
            
            if(activeUser == null)
            {
                return 3;
            }

            try
            {
                warehouse.name = (string)jsonResult.SelectToken("name");
                warehouse.code = (int)jsonResult.SelectToken("code");
                warehouse.description = (string)jsonResult.SelectToken("description");
                warehouse.created = DateTime.UtcNow;
                warehouse.id_user = activeUser.id_user;
                warehouse.User = activeUser;
            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(warehouse.name) || String.IsNullOrEmpty(warehouse.description))
            {
                return 0;
            }
            if (unitOfWork.WarehouseRepository.CheckIfExistWarehouseByName(warehouse.name))
            {
                return 1;
            }
            if (unitOfWork.WarehouseRepository.CheckIfExistWarehouseByCode(warehouse.code))
            {
                return 2;
            }

            try
            {
                unitOfWork.WarehouseRepository.Insert(warehouse);
                unitOfWork.Complete();
                return -1;
            }
            catch (Exception)
            {
                return 3;
            }
            
        }
        [HttpPut]
        [Route("Warehouses/{id}")]
        public int putWarehouse (int id, JObject jsonResult)
        {
            Warehouse warehouseForEdit = unitOfWork.WarehouseRepository.GetById(id);

            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                warehouseForEdit.code = (int)jsonResult.SelectToken("code");
                warehouseForEdit.name = (string)jsonResult.SelectToken("name");
                warehouseForEdit.description = (string)jsonResult.SelectToken("description");;
            }
            catch (Exception)
            {
                return 0;
            }

            if (String.IsNullOrEmpty(warehouseForEdit.name) || String.IsNullOrEmpty(warehouseForEdit.description))
            {
                return 0;
            }

            //check for other objects with the same code and name

            var warehouseList = unitOfWork.WarehouseRepository.GetAll();

            foreach (Warehouse w in warehouseList)
            {
               if(w.id_warehouse != id)
                {
                    if (w.name.Equals(warehouseForEdit.name))
                    {
                        return 1;
                    }
                    if (w.code == warehouseForEdit.code)
                    {
                        return 2;
                    }
                }
            }

            try
            {
                unitOfWork.WarehouseRepository.Update(warehouseForEdit);
                unitOfWork.Complete();

                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        /*Customer webapi*/
        [HttpGet]
        [Route("Customers")]
        public dynamic getCustomers()
        {
            var customers = unitOfWork.CustomerRepository.GetAllActiveCustomers();
            return customers;
        }
        [HttpGet]
        [Route("Customers/{id}")]
        public dynamic getCustomersForId(int id)
        {
            var customer = unitOfWork.CustomerRepository.GetActiveCustomerByID(id);
            return Json(customer);
        }
        [HttpDelete]
        [Route("Customers/{id}")]
        public void deleteCustomer(int id)
        {
            Customer customer = unitOfWork.CustomerRepository.GetById(id);
            if (customer != null)
            {
                try
                {
                    customer.is_active = false;
                    unitOfWork.CustomerRepository.Update(customer);
                    unitOfWork.Complete();
                }
                catch (Exception)
                {

                }
            }
        }
        [HttpPost]
        [Route("Customers")]
        public int postCustomers(JObject jsonResult)
        {
            Customer customer = new Customer();
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                customer.code = (int)jsonResult.SelectToken("code");
                customer.name = (string)jsonResult.SelectToken("name");
                customer.street = (string)jsonResult.SelectToken("street");
                customer.zipCode = (string)jsonResult.SelectToken("zipCode");
                customer.city = (string)jsonResult.SelectToken("city");
                customer.type = (int)jsonResult.SelectToken("type");
                customer.created = DateTime.UtcNow;
                customer.id_user = activeUser.id_user;
                customer.User = activeUser;
                customer.is_active = true;
            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(customer.name) || String.IsNullOrEmpty(customer.street) || String.IsNullOrEmpty(customer.zipCode) || String.IsNullOrEmpty(customer.city))
            {
                return 0;
            }
            if (unitOfWork.CustomerRepository.CheckIfExistActiveCustomerByName(customer.name))
            {
                return 1;
            }
            if (unitOfWork.CustomerRepository.CheckIfExistActiveCustomerByCode(customer.code))
            {
                return 2;
            }

            try
            {
                unitOfWork.CustomerRepository.Insert(customer);
                unitOfWork.Complete();

                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        [HttpPut]
        [Route("Customers/{id}")]
        public int putCustomer(int id, JObject jsonResult)
        {
            Customer customerForEdit = unitOfWork.CustomerRepository.GetById(id);
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                customerForEdit.code = (int)jsonResult.SelectToken("code");
                customerForEdit.name = (string)jsonResult.SelectToken("name");
                customerForEdit.street = (string)jsonResult.SelectToken("street");
                customerForEdit.zipCode = (string)jsonResult.SelectToken("zipCode");
                customerForEdit.city = (string)jsonResult.SelectToken("city");
                customerForEdit.type = (int)jsonResult.SelectToken("type");
            }
            catch (Exception)
            {
                return 0;
            }

            if (String.IsNullOrEmpty(customerForEdit.name) || String.IsNullOrEmpty(customerForEdit.street) || String.IsNullOrEmpty(customerForEdit.zipCode) || String.IsNullOrEmpty(customerForEdit.city))
            {
                return 0;
            }

            var customerList = unitOfWork.CustomerRepository.GetAll().Where(c => c.is_active == true);

            //check for other objects with the same code and name
            foreach (Customer c in customerList)
            {
                if (c.id_customer != id)
                {
                    if (c.name.Equals(customerForEdit.name))
                    {
                        return 1;
                    }
                    if (c.code == customerForEdit.code)
                    {
                        return 2;
                    }
                }
            }
            try
            {
                unitOfWork.CustomerRepository.Update(customerForEdit);
                unitOfWork.Complete();

                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        /*Group webapi*/
        [HttpGet]
        [Route("Groups")]
        public dynamic getGroups()
        {
            var groups = unitOfWork.GroupRepository.GetAllActiveGroup();
            return groups;
        }
        [HttpGet]
        [Route("Groups/{id}")]
        public dynamic getGroupsForId(int id)
        {
            var group = unitOfWork.GroupRepository.GetActiveGroupByID(id);
            return Json(group);
        }
        [HttpDelete]
        [Route("Groups/{id}")]
        public void deleteGroup(int id)
        {
            Group group = unitOfWork.GroupRepository.GetById(id);
            if (group != null)
            {
                try
                {
                    group.is_active = false;
                    unitOfWork.GroupRepository.Update(group);
                    unitOfWork.Complete();
                }
                catch (Exception)
                {

                }
            }
        }
        [HttpPost]
        [Route("Groups")]
        public int postGroups(JObject jsonResult)
        {

            Group group = new Group();
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                group.code = (int)jsonResult.SelectToken("code");
                group.name = (string)jsonResult.SelectToken("name");
                group.description = (string)jsonResult.SelectToken("description");
                group.is_active = true;
                group.created = DateTime.UtcNow;
                group.id_user = activeUser.id_user;
                group.User = activeUser;

            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(group.name) || String.IsNullOrEmpty(group.description))
            {
                return 0;
            }
            if (unitOfWork.GroupRepository.CheckIfExistActiveGroupByName(group.name))
            {
                return 1;
            }
            if (unitOfWork.GroupRepository.CheckIfExistActiveGroupByCode(group.code))
            {
                return 2;
            }

            try
            {
                unitOfWork.GroupRepository.Insert(group);
                unitOfWork.Complete();
                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        [HttpPut]
        [Route("Groups/{id}")]
        public int putGroup(int id, JObject jsonResult)
        {
            Group groupForEdit = unitOfWork.GroupRepository.GetById(id);
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                groupForEdit.code = (int)jsonResult.SelectToken("code");
                groupForEdit.name = (string)jsonResult.SelectToken("name");
                groupForEdit.description = (string)jsonResult.SelectToken("description");
            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(groupForEdit.name) || String.IsNullOrEmpty(groupForEdit.description))
            {
                return 0;
            }
            var groupList = unitOfWork.GroupRepository.GetAll().Where(g => g.is_active == true);

            //check for other objects with the same code and name

            foreach (Group g in groupList)
            {
                if (g.id_group != id)
                {
                    if (g.name.Equals(groupForEdit.name))
                    {
                        return 1;
                    }
                    if (g.code == groupForEdit.code)
                    {
                        return 2;
                    }
                }
            }
            try
            {
                unitOfWork.GroupRepository.Update(groupForEdit);
                unitOfWork.Complete();
                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }

        /*Product webapi*/        
        [HttpGet]
        [Route("Products")]
        public dynamic getProduct()
        {
            var products = unitOfWork.ProductRepository.GetAllActiveProducts();
            return products;
        }
        [HttpGet]
        [Route("AllProductsForWarehouse/{id}")]
        public dynamic getAllWithNotActiveProductsForWarehouse(int id)
        {
            var inventories = unitOfWork.InventoryRepository.GetAll().Where(i => i.id_warehouse == id && i.amount > 0).ToList();
            var products = unitOfWork.ProductRepository.GetAllWithNotActiveProducts();

            List<dynamic> product = new List<dynamic>(); 
            List<dynamic> productsToReturn = new List<dynamic>();

            foreach (Inventory i in inventories)
            {
                product = products.Select(row => row).Where(x => x.id_product == i.id_product).ToList();
                productsToReturn.AddRange(product);
            }

            return productsToReturn.AsEnumerable();
        }
        [HttpGet]
        [Route("ProductsWithNoActive/{id}")]
        public dynamic getAllWithNotActiveProducts(int id)
        {
            var products = unitOfWork.ProductRepository.GetProductByID(id);
            return products;
        }
        [Route("Products/{id}")]
        public dynamic getProductsForId(int id)
        {
            var product = unitOfWork.ProductRepository.GetActiveProductByID(id);
            return Json(product);
        }
        [HttpDelete]
        [Route("Products/{id}")]
        public void deleteProduct(int id)
        {
            Product product = unitOfWork.ProductRepository.GetById(id);
            if (product != null)
            {
                try
                {
                    product.is_active = false;
                    unitOfWork.ProductRepository.Update(product);
                    unitOfWork.Complete();
                }
                catch (Exception)
                {

                }

            }
        }
        [HttpPost]
        [Route("Products")]
        public int postProduct(JObject jsonResult)
        {

            Product product = new Product();
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                product.id_group = (int)jsonResult.SelectToken("groupType");
                product.code = (int)jsonResult.SelectToken("code");
                product.name = (string)jsonResult.SelectToken("name");
                product.unit = (int)jsonResult.SelectToken("unitType");
                product.description = (string)jsonResult.SelectToken("description");
                product.is_active = true;
                product.created = DateTime.UtcNow;
                product.id_user = activeUser.id_user;
                product.User = activeUser;
                product.Group = unitOfWork.GroupRepository.GetById(product.id_group);

            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(product.name) || String.IsNullOrEmpty(product.description))
            {
                return 0;
            }
            if (unitOfWork.ProductRepository.CheckIfExistActiveProductByName(product.name))
            {
                return 1;
            }
            if (unitOfWork.ProductRepository.CheckIfExistActiveProductByCode(product.code))
            {
                return 2;
            }

            try
            {
                unitOfWork.ProductRepository.Insert(product);
                unitOfWork.Complete();

                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        [HttpPut]
        [Route("Products/{id}")]
        public int putProduct(int id, JObject jsonResult)
        {
            Product productForEdit = unitOfWork.ProductRepository.GetById(id);
            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            try
            {
                productForEdit.id_group = (int)jsonResult.SelectToken("groupType");
                productForEdit.code = (int)jsonResult.SelectToken("code");
                productForEdit.name = (string)jsonResult.SelectToken("name");
                productForEdit.unit = (int)jsonResult.SelectToken("unitType");
                productForEdit.description = (string)jsonResult.SelectToken("description");
                productForEdit.Group = unitOfWork.GroupRepository.GetById(productForEdit.id_group);
            }
            catch (Exception)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(productForEdit.name) || String.IsNullOrEmpty(productForEdit.description))
            {
                return 0;
            }
            var productList = unitOfWork.ProductRepository.GetAll().Where(g => g.is_active == true);

            foreach (Product p in productList)
            {
                if (p.id_product != id)
                {
                    if (p.name.Equals(productForEdit.name))
                    {
                        return 1;
                    }
                    if (p.code == productForEdit.code)
                    {
                        return 2;
                    }
                }
            }
            try
            {
                unitOfWork.ProductRepository.Update(productForEdit);
                unitOfWork.Complete();
                return -1;
            }
            catch (Exception)
            {
                return 3;
            }

        }
        /*Moves*/
        [HttpPost]
        [Route("Moves")]
        public int postMoves(JObject jsonResult)
        {

            User activeUser = getActiveUser();

            if (activeUser == null)
            {
                return 3;
            }

            Move move = new Move();

            move.type = (int)jsonResult.SelectToken("type");

            move.id_warehouse1 = (int)jsonResult.SelectToken("warehouseOne");
            move.time = (DateTime)jsonResult.SelectToken("date");
            move.id_user = activeUser.id_user;
            move.number = unitOfWork.MoveRepository.GetNumberOfDocuments(move.type);
            move.WarehouseOne = unitOfWork.WarehouseRepository.GetById(move.id_warehouse1);
            move.User = activeUser;

            if (move.type != 3)
            {
                move.id_warehouse2 = 2;
                move.id_custmer = (int)jsonResult.SelectToken("customer");
                move.Customer = unitOfWork.CustomerRepository.GetById(move.id_custmer);
            }
            else
            {
                move.id_custmer = 0;
                move.id_warehouse2 = (int)jsonResult.SelectToken("customer");
                move.WarehouseTwo = unitOfWork.WarehouseRepository.GetById(move.id_warehouse2);
            }

            move.Inventories = inventoryOperations(jsonResult, move.id_warehouse1, move.id_warehouse2, move.type);
            try
            {
                unitOfWork.MoveRepository.Insert(move);
                unitOfWork.Complete();
            }
            catch (Exception)
            {
                return 3;
            }

            //generate pdf document
            PdfDocument pdfDocument = new PdfDocument();
            byte[] moveDocument = pdfDocument.preparePdf(move);
            pdfDocument.savePDF(moveDocument, move);

            return 1;
        }
        /*inventories*/
        [HttpGet]
        [Route("Inventories")]
        public dynamic getInventories()
        {
            var inventories = unitOfWork.InventoryRepository.GetAllInventories();
            return inventories;
        }
        [HttpGet] 
        [Route("Inventories/{id}")]
        public dynamic getInventoryForId(int id)
        {
            var inventory = unitOfWork.InventoryRepository.GetInventoryrByID(id);
            return Json(inventory);
        }
        [HttpPost]
        [Route("Inventories")]
        public dynamic getAllproductsFromWarehouse(JObject jsonResult)
        {
            int idWarehouse = (int)jsonResult.SelectToken("idWarehouse");
            int idProduct = (int)jsonResult.SelectToken("idProduct");

            var inventories = unitOfWork.InventoryRepository.GetAllproductsFromWarehouse(idWarehouse, idProduct);

            return Json(inventories);
        }
        /*documents*/
        [HttpGet]
        [Route("Document")]
        public dynamic getDocuments()
        {
            var documents = unitOfWork.MoveRepository.GetAllDocuments();
            return Json(documents);
        }
        public List<Inventory> inventoryOperations(JObject inventoriesArray, int idWarehouseOne , int idWarehouseTwo, int movementType)
        {

            List<Inventory> inventories = new List<Inventory>();
            Inventory inventory = new Inventory(); 
            string producetName;

            var inventoriesList = unitOfWork.InventoryRepository.GetAll();
            
            foreach (var product in inventoriesArray["products"])
            {
                var inventoryFromJson = new Inventory();
                producetName = (string)product["productName"];
                inventoryFromJson.id_warehouse = idWarehouseOne;
                inventoryFromJson.id_product = (int)product["productId"];
                inventoryFromJson.Product = unitOfWork.ProductRepository.GetById(inventoryFromJson.id_product);
                inventoryFromJson.amount = (int)product["amount"];
                inventories.Add(inventoryFromJson);
            }

            foreach (var i in inventories) {

                var updateInventory = unitOfWork.InventoryRepository.GetInventoryByIdProductAndIdWarehouse(i.id_product, i.id_warehouse);
               
                if (movementType == 1)
                {
                    addQuantityToInventory(i, updateInventory);
                }
                else if (movementType == 2)
                {
                    subtractQuantitfromInventory(i, updateInventory);
                }
                else
                {
                    transferQuantityBetweenInventory(i, updateInventory, idWarehouseTwo);
                }
            }
            return inventories;
        }

        public void subtractQuantitfromInventory(Inventory inventory, Inventory updatedInventory)
        {
            if (updatedInventory != null)
            {
                updatedInventory.amount -= inventory.amount;
                unitOfWork.InventoryRepository.Update(updatedInventory);
                unitOfWork.Complete();
            }
        }

        public void addQuantityToInventory(Inventory inventory, Inventory updatedInventory)
        {
            if (updatedInventory != null)
            {
                updatedInventory.amount += inventory.amount;
                unitOfWork.InventoryRepository.Update(updatedInventory);
                unitOfWork.Complete();
            }
            else
            {
                unitOfWork.InventoryRepository.Insert(inventory);
                unitOfWork.Complete();
            }
        }

        public void transferQuantityBetweenInventory(Inventory inventory, Inventory updatedInventory, int idWarehouseTwo)
        {
            var updatedInventoryTwo = unitOfWork.InventoryRepository.GetInventoryByIdProductAndIdWarehouse(inventory.id_product, idWarehouseTwo);
            
            if(updatedInventoryTwo != null)
            {
                updatedInventory.amount -= inventory.amount;
                updatedInventoryTwo.amount += inventory.amount;

                unitOfWork.InventoryRepository.Update(updatedInventory);
                unitOfWork.InventoryRepository.Update(updatedInventoryTwo);
                unitOfWork.Complete();
            }
            else
            {
                updatedInventory.amount -= inventory.amount;
                //add inventory to second warehouse
                inventory.id_warehouse = idWarehouseTwo;
                unitOfWork.InventoryRepository.Update(updatedInventory);
                unitOfWork.InventoryRepository.Insert(inventory);
                unitOfWork.Complete();
            }
        }
    }

}

    

 