using System;
using System.Linq;
using System.Web.Http;
using Magazyn2019.Models;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Magazyn2019.Controllers
{
    //zamienic pobieranie uzytkowniak w jedna funkcje, bo jest zbyt wiele razy wywoływana
    [RoutePrefix("")]
    public class MagazynApiController : ApiController
    {
        private Magazyn2019Entities db = new Magazyn2019Entities();

        [HttpPost]
        [Route("login")]
        public int CheckLogin(JObject jsonResult)
        {
            string login_txt = (string)jsonResult.SelectToken("login_txt");
            string password_txt = (string)jsonResult.SelectToken("password_txt");

            var loginResult = db.Users.Where(x => login_txt == x.userName && password_txt == x.password).FirstOrDefault();
            if (loginResult != null)
            {
              getActiveUser(login_txt);
              return 1;
            }
            else
            {
              return 0;
            }
        }
        [HttpPost]
        [Route("logout")]
        public void Logout()
        {
            HttpContext.Current.Session["ActiveUserId"] = null;
        }
        public void getActiveUser(string login_txt)
        {

            User activeUser = db.Users.Single(x => x.userName == login_txt);

            HttpContext.Current.Session["ActiveUserId"] = activeUser.id_user;
            HttpContext.Current.Session["ActiveUserFullName"] = activeUser.fullName;
            HttpContext.Current.Session["ActiveUserPosition"] = activeUser.position;
            
        }
        /*użyto tego w celu uniknięcia przetworzenia w json'a całego modelu tj. relacji z nim powiązanych */
        [HttpGet]
        [Route("Warehouses")]
        public dynamic getWarehouses()
        {
            var wareHouses = db.Warehouses.Select(x =>
            new {x.id_warehouse, x.code, x.name, x.description, x.created, x.id_user});
            return wareHouses;
        }
        [HttpGet]
        [Route("Warehouses/{id}")]
        public dynamic getWarehouseForId(int id)
        {
            var warehouse = from w in db.Warehouses
                            where id == w.id_warehouse
                            select new
                            {
                                name = w.name,
                                code = w.code,
                                description = w.description,
                                created =  w.created,
                                userName = w.User.fullName,
                            };
            return Json(warehouse);
        }
        [HttpDelete]
        [Route("Warehouses/{id}")]
        public void deleteWarehouse(int id)
        {
            Warehouse warehouse = db.Warehouses.Where(x => x.id_warehouse == id).FirstOrDefault();
            if (warehouse != null)
            {
                db.Warehouses.Remove(warehouse);
                db.SaveChanges();
            }
        }
        [HttpPost]
        [Route("Warehouses")]
        public int postWarehouse(JObject jsonResult)
        {

            Warehouse warehouse = new Warehouse();

            var id = HttpContext.Current.Session["ActiveUserId"];
            int id_user=(int)id;
            User activeUser = db.Users.Single(x => x.id_user == id_user);

            try
            {
                warehouse.name = (string)jsonResult.SelectToken("name");
                warehouse.code = (int)jsonResult.SelectToken("code");
                warehouse.description = (string)jsonResult.SelectToken("description");
                warehouse.created = DateTime.UtcNow;
                warehouse.id_user = activeUser.id_user;
                warehouse.User = activeUser;
            }
            catch (System.FormatException e)
            {
                return 0;
            }
            if (db.Warehouses.Any(x => x.name == warehouse.name))
            {
                return 1;
            }
            if (db.Warehouses.Any(x => x.code == warehouse.code))
            {
                return 2;
            }

            db.Warehouses.Add(warehouse);
            db.SaveChanges();
            return -1;
        }
        [HttpPut]
        [Route("Warehouses/{id}")]
        public int putWarehouse (int id, JObject jsonResult)
        {
            Warehouse warehouse = new Warehouse();
            try
            {
                warehouse.code = (int)jsonResult.SelectToken("code");
                warehouse.name = (string)jsonResult.SelectToken("name");
                warehouse.description = (string)jsonResult.SelectToken("description");;
            }
            catch (System.FormatException e)
            {
                return 0;
            }


            var warehouseList = from w in db.Warehouses
                                  select w;

            foreach (Warehouse w in warehouseList)
            {
               if(w.id_warehouse != id)
                {
                    if (w.name.Equals(warehouse.name))
                    {
                        return 1;
                    }
                    if (w.code == warehouse.code)
                    {
                        return 2;
                    }
                }
            }
               Warehouse warehouseEdit = db.Warehouses.Single(x => x.id_warehouse == id);
               warehouseEdit.code = warehouse.code;
               warehouseEdit.name = warehouse.name;
               warehouseEdit.description = warehouse.description;
               db.SaveChanges();
               return -1;
        }
        /*Customer webapi*/
        [HttpGet]
        [Route("Customers")]
        public dynamic getCustomers()
        {
            var customers = db.Customers.Select(x =>
            new { x.id_customer, x.name, x.code, x.street, x.zipCode, x.city, x.type, x.created, x.id_user, x.is_active }).Where(x => x.is_active ==true);
            return customers;
        }
        [HttpGet]
        [Route("Customers/{id}")]
        public dynamic getCustomersForId(int id)
        {
            var customer = from c in db.Customers
                            where id == c.id_customer 
                            && c.is_active == true
                            select new
                            {
                                name = c.name,
                                code = c.code,
                                street = c.street,
                                zipCode = c.zipCode,
                                city = c.city,
                                type = c.type,
                                created = c.created,
                                userName = c.User.fullName,
                            };
            return Json(customer);
        }
        [HttpDelete]
        [Route("Customers/{id}")]
        public void deleteCustomer(int id)
        {
            Customer customer = db.Customers.Where(x => x.id_customer == id).FirstOrDefault();
            if (customer != null)
            {
                customer.is_active = false;
                //db.Customers.Remove(customer);
                db.SaveChanges();
            }
        }
        [HttpPost]
        [Route("Customers")]
        public int postCustomers(JObject jsonResult)
        {

            Customer customer = new Customer();

            var id = HttpContext.Current.Session["ActiveUserId"];
            int id_user = (int)id;
            User activeUser = db.Users.Single(x => x.id_user == id_user);


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
            catch (System.FormatException e)
            {
                return 0;
            }
            if (db.Customers.Any(x => x.name == customer.name && x.is_active == true))
            {
                return 1;
            }
            if (db.Customers.Any(x => x.code == customer.code && x.is_active == true))
            {
                return 2;
            }

            db.Customers.Add(customer);
            db.SaveChanges();
            return -1;
        }
        [HttpPut]
        [Route("Customers/{id}")]
        public int putCustomer(int id, JObject jsonResult)
        {
            Customer customer = new Customer();
            try
            {
                customer.code = (int)jsonResult.SelectToken("code");
                customer.name = (string)jsonResult.SelectToken("name");
                customer.street = (string)jsonResult.SelectToken("street");
                customer.zipCode = (string)jsonResult.SelectToken("zipCode");
                customer.city = (string)jsonResult.SelectToken("city");
                customer.type = (int)jsonResult.SelectToken("type");
            }
            catch (System.FormatException e)
            {
                return 0;
            }


            var customerList = from c in db.Customers
                                where c.is_active == true
                                select c;


            foreach (Customer c in customerList)
            {
                if (c.id_customer != id)
                {
                    if (c.name.Equals(customer.name))
                    {
                        return 1;
                    }
                    if (c.code == customer.code)
                    {
                        return 2;
                    }
                }
            }
            Customer customerEdit = db.Customers.Single(x => x.id_customer == id);
            customerEdit.code = customer.code;
            customerEdit.name = customer.name;
            customerEdit.street = customer.street;
            customerEdit.zipCode = customer.zipCode;
            customerEdit.city = customer.city;
            customerEdit.type = customer.type;
            db.SaveChanges();
            return -1;
        }
        /*Group webapi*/
        [HttpGet]
        [Route("Groups")]
        public dynamic getGroup()
        {
            var groups = db.Groups.Select(x =>
            new { x.id_group, x.name, x.code,x.description, x.created, x.id_user, x.is_active }).Where(x => x.is_active == true);
            return groups;
        }
        [HttpGet]
        [Route("Groups/{id}")]
        public dynamic getGroupsForId(int id)
        {
            var group = from g in db.Groups
                           where id == g.id_group && g.is_active == true
                           select new
                           {
                               name = g.name,
                               code = g.code,
                               created = g.created,
                               description = g.description,
                               userName = g.User.fullName,
                           };
            return Json(group);
        }
        [HttpDelete]
        [Route("Groups/{id}")]
        public void deleteGroup(int id)
        {
            Group group = db.Groups.Where(x => x.id_group == id).FirstOrDefault();
            if (group != null)
            {
                group.is_active = false;
                db.SaveChanges();
            }
        }
        [HttpPost]
        [Route("Groups")]
        public int postGroups(JObject jsonResult)
        {

            Group group = new Group();

            var id = HttpContext.Current.Session["ActiveUserId"];
            int id_user = (int)id;
            User activeUser = db.Users.Single(x => x.id_user == id_user);

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
            catch (System.FormatException e)
            {
                return 0;
            }
            if (db.Groups.Any(x => x.name == group.name && x.is_active == true))
            {
                return 1;
            }
            if (db.Groups.Any(x => x.code == group.code && x.is_active == true))
            {
                return 2;
            }

            db.Groups.Add(group);
            db.SaveChanges();
            return -1;
        }
        [HttpPut]
        [Route("Groups/{id}")]
        public int putGroup(int id, JObject jsonResult)
        {
            Group group = new Group();
            try
            {
                group.code = (int)jsonResult.SelectToken("code");
                group.name = (string)jsonResult.SelectToken("name");
                group.description = (string)jsonResult.SelectToken("description");
            }
            catch (System.FormatException e)
            {
                return 0;
            }


            var groupList = from g in db.Groups
                            where g.is_active == true
                            select g;

            foreach (Group g in groupList)
            {
                if (g.id_group != id)
                {
                    if (g.name.Equals(group.name))
                    {
                        return 1;
                    }
                    if (g.code == group.code)
                    {
                        return 2;
                    }
                }
            }
            Group groupEdit = db.Groups.Single(x => x.id_group == id);
            groupEdit.code = group.code;
            groupEdit.name = group.name;
            db.SaveChanges();
            return -1;
        }

        /*Product webapi*/
        
        [HttpGet]
        [Route("Products")]
        public dynamic getProduct()
        {
            var products = db.Products.Select(x =>
            new { x.id_product, x.id_group,x.name, x.code, x.description, x.unit, x.created, x.id_user, group_name = x.Group.name, x.is_active }).Where( x => x.is_active == true);
            return products;
        }
        [HttpGet]
        [Route("Products/{id}")]
        public dynamic getProductsForId(int id)
        {
            var product = from p in db.Products
                        where id == p.id_product && p.is_active == true
                        select new
                        {
                            id_group = p.id_group,
                            code = p.code,
                            name = p.name,
                            description = p.description,
                            unit = p.unit,
                            created = p.created,
                            group_name = p.Group.name,
                            userName = p.User.fullName,
                        };
            return Json(product);
        }
        [HttpDelete]
        [Route("Products/{id}")]
        public void deleteProduct(int id)
        {
            Product product = db.Products.Where(x => x.id_product == id).FirstOrDefault();
            if (product != null)
            {
                product.is_active = false;
                db.SaveChanges();
            }
        }
        [HttpPost]
        [Route("Products")]
        public int postProduct(JObject jsonResult)
        {

            Product product = new Product();

            var id = HttpContext.Current.Session["ActiveUserId"];
            int id_user = (int)id;
            User activeUser = db.Users.Single(x => x.id_user == id_user);

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
                product.Group = db.Groups.Single(x => x.id_group == product.id_group);

            }
            catch (System.FormatException e)
            {
                return 0;
            }
            if (db.Products.Any(x => x.name == product.name && x.is_active == true))
            {
                return 1;
            }
            if (db.Products.Any(x => x.code == product.code && x.is_active == true))
            {
                return 2;
            }

            db.Products.Add(product);
            db.SaveChanges();
            return -1;
        }
        [HttpPut]
        [Route("Products/{id}")]
        public int putProduct(int id, JObject jsonResult)
        {
            Product product = new Product();
            try
            {
                product.id_group = (int)jsonResult.SelectToken("groupType");
                product.code = (int)jsonResult.SelectToken("code");
                product.name = (string)jsonResult.SelectToken("name");
                product.unit = (int)jsonResult.SelectToken("unitType");
                product.description = (string)jsonResult.SelectToken("description");
                product.Group = db.Groups.Single(x => x.id_group == product.id_group);
            }
            catch (System.FormatException e)
            {
                return 0;
            }

            var productList = from p in db.Products
                              where p.is_active == true
                              select p;

            foreach (Product p in productList)
            {
                if (p.id_product != id)
                {
                    if (p.name.Equals(product.name))
                    {
                        return 1;
                    }
                    if (p.code == product.code)
                    {
                        return 2;
                    }
                }
            }
            Product productEdit = db.Products.Single(x => x.id_product == id);
            productEdit.id_group = product.id_group;
            productEdit.code = product.code;
            productEdit.name = product.name;
            productEdit.unit = product.unit;
            productEdit.description = product.description;
            product.Group = db.Groups.Single(x => x.id_group == product.id_group);
            db.SaveChanges();
            return -1;
        }
        /*Moves*/
        [HttpPost]
        [Route("Moves")]
        public void postMoves(JObject jsonResult)
        {
            var id = HttpContext.Current.Session["ActiveUserId"];
            int id_user = (int)id;

            Move move = new Move();

            move.type = (int)jsonResult.SelectToken("type");

            move.id_warehouse1 = (int)jsonResult.SelectToken("warehouseOne");
            move.time = (DateTime)jsonResult.SelectToken("date");
            move.id_user = id_user;
            move.number = getNumberOfDocuments(move.type);
            move.WarehouseOne = db.Warehouses.Single(x => x.id_warehouse == move.id_warehouse1);
            move.User = db.Users.Single(x => x.id_user == move.id_user);

            if (move.type != 3)
            {
                move.id_warehouse2 = 2;
                move.id_custmer = (int)jsonResult.SelectToken("customer");
                move.Customer = db.Customers.Single(x => x.id_customer == move.id_custmer);
            }
            else
            {
                move.id_custmer = 0;
                move.id_warehouse2 = (int)jsonResult.SelectToken("customer");
                move.WarehouseTwo = db.Warehouses.Single(x => x.id_warehouse == move.id_warehouse2);
            }

            //move.Inventories = addInventories(jsonResult, move.id_warehouse1, move.id_warehouse2, move.type);
            move.Inventories = inventoryOperations(jsonResult, move.id_warehouse1, move.id_warehouse2, move.type);


            db.Moves.Add(move);
            db.SaveChanges();

            PdfDocument pdfDocument = new PdfDocument();
            byte[] moveDocument = pdfDocument.preparePdf(move);
            pdfDocument.savePDF(moveDocument, move);
        }
        /*inventories*/
        [HttpGet]
        [Route("Inventories")]
        public dynamic getInventories()
        {
            var invetories = db.Inventories.Select(x =>
            new { x.id_inventory, x.id_product, x.id_warehouse, x.amount});
            return invetories;
        }
        [HttpGet]
        [Route("Inventories/{id}")]
        public dynamic getInventoryForId(int id)
        {
            var inventory = from i in db.Inventories
                            where id == i.id_warehouse && i.amount > 0
                            select new 
                            {
                                id_inventory = i.id_inventory,
                                id_product = i.id_product,
                                id_warehouse = i.id_warehouse,
                                amount = i.amount,
                                name = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.name),
                                code = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.code),
                                groupName = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.Group.name),
                                description = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.description),
                                unit = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.unit),
                            };
            return Json(inventory);
        }
        [HttpPost]
        [Route("Inventories")]
        public dynamic getAllproductsFromWarehouse(JObject jsonResult)
        {
            int idWarehouse = (int)jsonResult.SelectToken("idWarehouse");
            int idProduct = (int)jsonResult.SelectToken("idProduct");

            var inventories = from i in db.Inventories
                              where (idWarehouse == i.id_warehouse)
                              && (idProduct == i.id_product)
                              select new
                              {
                                  id_inventory = i.id_inventory,
                                  id_product = i.id_product,
                                  id_warehouse = i.id_warehouse,
                                  amount = i.amount,
                                  name = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.name),
                                  code = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.code),
                                  groupName = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.Group.name),
                                  description = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.description),
                                  unit = db.Products.Where(x => x.id_product == i.id_product).Select(x => x.unit),
                              };
            return Json(inventories);
        }
        /*documents*/
        [HttpGet]
        [Route("Document")]
        public dynamic getDocuments()
        {
            var documents = db.Moves.Select(x =>
            new
            {
                x.id_move,
                x.type,
                x.number,
                x.time,
                name_WarehouseOne = db.Warehouses.Where(w => w.id_warehouse == x.id_warehouse1).Select(w => w.name),
                name_WarehouseTwo = db.Warehouses.Where(w => w.id_warehouse == x.id_warehouse2).Select(w => w.name),
                name_User = db.Users.Where(u => u.id_user == x.id_user).Select(u => u.fullName),
                name_Customer = db.Customers.Where(c => c.id_customer == x.id_custmer).Select(c => c.name),
            });

            return Json(documents);
        }
        public List<Inventory> inventoryOperations(JObject inventoriesArray, int idWarehouseOne , int idWarehouseTwo, int movementType)
        {

            List<Inventory> inventories = new List<Inventory>();
            Inventory inventory = new Inventory(); ;
            string producetName;

            var inventoriesList = from i in db.Inventories
                                  select i;
            
            foreach (var product in inventoriesArray["products"])
            {
                var inventoryFromJson = new Inventory();
                producetName = (string)product["productName"];
                inventoryFromJson.id_warehouse = idWarehouseOne;
                inventoryFromJson.id_product = (int)product["productId"];
                inventoryFromJson.Product = db.Products.SingleOrDefault(p => p.id_product == inventoryFromJson.id_product);
                inventoryFromJson.amount = (int)product["amount"];
                inventories.Add(inventoryFromJson);
            }

            foreach (var i in inventories) {

                var updateInventory = db.Inventories.SingleOrDefault(x => x.id_product == i.id_product && x.id_warehouse == i.id_warehouse);

               
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
                db.SaveChanges();
            }
        }

        public void addQuantityToInventory(Inventory inventory, Inventory updatedInventory)
        {
            if (updatedInventory != null)
            {
                updatedInventory.amount += inventory.amount;
                db.SaveChanges();
            }
            else
            {
                db.Inventories.Add(inventory);
                db.SaveChanges();
            }
        }

        public void transferQuantityBetweenInventory(Inventory inventory, Inventory updatedInventory, int idWarehouseTwo)
        {
            var updatedInventoryTwo = db.Inventories.SingleOrDefault(x => x.id_product == inventory.id_product && x.id_warehouse == idWarehouseTwo);
            
            if(updatedInventoryTwo != null)
            {
                updatedInventory.amount -= inventory.amount;
                updatedInventoryTwo.amount += inventory.amount;
                db.SaveChanges();
            }
            else
            {
                updatedInventory.amount -= inventory.amount;
                //add inventory to second warehouse
                inventory.id_warehouse = idWarehouseTwo;
                db.Inventories.Add(inventory);
                db.SaveChanges();
            }
        }

        /*
        public List<Inventory> addInventories(JObject inventoriesArray, int idWarehouseOne, int idWarehouseTwo, int movementType)
        {

            List<Inventory> inventories = new List<Inventory>();

            Inventory inventory = new Inventory(); ;

            string producetName;
            var inventoriesList = from i in db.Inventories
                                  select i;

            foreach (var product in inventoriesArray["products"])
            {
                var inventoryFromJson = new Inventory();
                producetName = (string)product["productName"];
                inventoryFromJson.id_warehouse = idWarehouseOne;
                inventoryFromJson.id_product = (int)product["productId"];
                inventoryFromJson.Product = db.Products.SingleOrDefault(p => p.id_product == inventoryFromJson.id_product);
                inventoryFromJson.amount = (int)product["amount"];
                inventories.Add(inventoryFromJson);
            }

            System.Diagnostics.Debug.WriteLine(movementType);

            foreach (var i in inventories)
            {

                var updateInventory = db.Inventories.SingleOrDefault(x => x.id_product == i.id_product && x.id_warehouse == i.id_warehouse);


                if (movementType == 1)
                {
                    if (updateInventory != null)
                    {
                        updateInventory.amount += i.amount;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Inventories.Add(i);
                        db.SaveChanges();
                    }
                }
                else if (movementType == 2)
                {
                    if (updateInventory != null)
                    {
                        updateInventory.amount -= i.amount;
                        db.SaveChanges();
                    }
                }
                else
                {
                    var updateInventoryTwo = db.Inventories.SingleOrDefault(x => x.id_product == i.id_product && x.id_warehouse == idWarehouseTwo);

                    updateInventory.amount -= i.amount;
                    updateInventoryTwo.amount += i.amount;

                    db.SaveChanges();
                }
            }
            return inventories;
        }
        */
        public int getNumberOfDocuments(int type)
        {
                var numberOfDocument = (from m in db.Moves
                                        where m.type == type
                                        select m ).Max(m => (int?)m.number) ?? 0;

                if (numberOfDocument == 0)
                {
                    numberOfDocument = 1;
                }
                else
                {
                    numberOfDocument++;
                }
                return numberOfDocument;

        }

        


    }

}

    

 