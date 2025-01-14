﻿using Newtonsoft.Json;

namespace Microsoft.eShopWeb.ApplicationCore.Entities;

// This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
// Using non-generic integer types for simplicity and to ease caching logic
public abstract class BaseEntity
{
    //[JsonProperty("id")]
    //public virtual string Id { get; set; }
    public virtual int Id { get; set; }
}
