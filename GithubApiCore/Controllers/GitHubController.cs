using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using GithubApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GithubApiCore.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class GitHubController : ControllerBase
  {
    [HttpGet]
    public ActionResult Get()
    {
      try
      {
        return Ok(IntegrarAPIGitHub());
      }
      catch (Exception)
      {
        return BadRequest();
      }
    }

    public List<ReposModel> IntegrarAPIGitHub()
    {
      try
      {
        //var _configuration = ConfigurationManager.AppSettings;

        List<ReposModel> repositorio = new List<ReposModel>();
        using (var client = new HttpClient())
        {
          client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

          var url = string.Format("https://api.github.com/orgs/takenet/repos");

          HttpResponseMessage response = client.GetAsync(url).Result;

          response.EnsureSuccessStatusCode();
          string conteudo = response.Content.ReadAsStringAsync().Result;

          dynamic resultado = JsonConvert.DeserializeObject(conteudo);

          foreach (var item in resultado)
          {
            var repos = new ReposModel
            {
              NomeCompleto = item.full_name,
              DescricaoRepositorio = item.description,
              Imagem = item.owner.avatar_url,
              DataCriacao = item.created_at,
              linguagemRepositorio = item.language.ToString()
            };

            repositorio.Add(repos);
          }
        }
        return repositorio.Where(b => b.linguagemRepositorio.Equals("C#")).OrderByDescending(a => a.DataCriacao).Take(5).ToList();
        //return Ok(repositorio.Where(b => b.linguagemRepositorio.Equals("C#")).OrderByDescending(a => a.DataCriacao).Take(5).ToList());
      }
      catch (Exception e)
      {
        return null;
      }
    }
  }
}
