using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TaskWebAPI.Models;

namespace TaskWebAPI.Controllers
{
    public class TaskController : ApiController
    {
        private TaskDB db = new TaskDB();

        // GET: api/Task
        public IQueryable<Task> GetTasks()
        {
            return db.Tasks;
        }

        // GET: api/Task/5
        [ResponseType(typeof(Task))]
        public IHttpActionResult GetTask(int id)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }
        // GET: api/Task?name=testing3
        public IQueryable<Task> GetTaskName(String name)
        {
            return db.Tasks.Where(task => task.TaskName == name);
          
        }

        // GET api/Task?doneBy={doneBy}
        public IQueryable<Task> GetTaskDoneBy(String doneBy)
        {
            return db.Tasks.Where(task => task.DoneBy == doneBy);

        }

        // GET api/Task?postedDate={postedDate}
        public IQueryable<Task> GetTaskPostedDate(String postedDate)
        {
            return db.Tasks.Where(task => task.PostedDate == postedDate);

        }
        /*public int GetTaskCountPostedDate(String postedDate)
        {
            return db.Tasks.Count(task => task.PostedDate == postedDate);

        }*/

        // PUT api/Task/{id}?postedDate={postedDate}
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTask(int id, string postedDate, Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != task.TaskID || postedDate != task.PostedDate)
            {
                return BadRequest();
            }

            db.Entry(task).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException )
            {
                if (!TaskExists(id,postedDate))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            } 
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Task
        [ResponseType(typeof(Task))]
        public IHttpActionResult PostTask(Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tasks.Add(task);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task.TaskID, task.PostedDate))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        Console.WriteLine(message);
                    }
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = task.TaskID }, task);
        }

        // DELETE api/Task/{id}?postedDate={postedDate}
        [ResponseType(typeof(Task))]
        public IHttpActionResult DeleteTask(int id, string postedDate)
        {
            Task task = db.Tasks.Find(id,postedDate);
            if (task == null)
            {
                return NotFound();
            }

            db.Tasks.Remove(task);
            db.SaveChanges();

            return Ok(task);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskExists(int id, string postedDate)
        {
            return db.Tasks.Count(e => e.TaskID == id && e.PostedDate == postedDate) > 0;
        }
    }
}