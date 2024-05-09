// 1 - Bibliotecas
using System.Runtime.Versioning;
using Models;
using Newtonsoft.Json; // dependencia para JsonConvert
using Newtonsoft.Json.Linq;
using RestSharp;

// 2 - NameSpace
namespace Pet;

// 3 - Classe
public class PetTest
{
// 3.1 - Atributos
// Endereço da API
private const string BASE_URL = "https://petstore.swagger.io/v2/";

// 3.2 Funções e Métodos

//Função de leitura de dados a partir de um arqivo csv
 public static IEnumerable<TestCaseData> getTestData()
    {
        String caminhoMassa = @"C:\ITERASYS\PetStore139\fixtures\Pets.csv";

        using var reader = new StreamReader(caminhoMassa);

        // Pula a primeira linha com os cabeçalhos
        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(",");

            yield return new TestCaseData(int.Parse(values[0]), int.Parse(values[1]), values[2], values[3], values[4], values[5], values[6], values[7]);
        }

}

[Test, Order(1)]
public void PostPetTest()
{
// Configura
// Instancia o objeto do tipo RestClient com o endereço da API
var client = new RestClient(BASE_URL);

// instancia o objeto do tipo RestRequest com o complemento de endereço
// como "pet" e configurando o metodo para ser um post (inclusao)
var request = new RestRequest("pet", Method.Post);

// armazena o conteudo do arquivo pet.json na memoria
String jsonBody = File.ReadAllText(@"C:\ITERASYS\PetStore139\fixtures\Pets.csv"); 

// adiciona na requisicao o conteudo do arquivo pet1.json
request.AddBody(jsonBody);

// Executa
// executa a requisição conforme a configuração realizada
// guarda o json retornando no objeto response
var response = client.Execute(request);

// Valida
var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

// Exibe o responseBody no console
Console.WriteLine(responseBody);


// Valide que na resposta, o status code é igual ao resultado esperado (200)
Assert.That((int)response.StatusCode, Is.EqualTo(200));

// Valida o petId 
int petId = responseBody.id;
Assert.That(petId, Is.EqualTo(3269347));

// Valida o nome do animal na resposta
String name = responseBody.name.ToString();
Assert.That(name, Is.EqualTo("Serafina"));

// Valida o status do animal na resposta
String status = responseBody.status;
Assert.That(status, Is.EqualTo("available"));

// Armazenar os dados obtidos para usar nos proximos testes
Environment.SetEnvironmentVariable("petId", petId.ToString()); 

}

[Test, Order(2)]
public void GetPetTest()
{

// Configura
int petId = 3269347;   // Campo de pesquisa
String petName = "Serafina";    // Resultado esperado
String categoryName = "Cat";
String tagsName = "vacinada";

var client = new RestClient(BASE_URL);
var request = new RestRequest($"pet/{petId}", Method.Get);

// Executa
var response = client.Execute(request);

// Valida
var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
Console.WriteLine(responseBody);

Assert.That((int)response.StatusCode, Is.EqualTo(200));
Assert.That((int)responseBody.Id, Is.EqualTo(petId));
Assert.That((String)responseBody.name, Is.EqualTo(petName));
Assert.That((String)responseBody.category.name, Is.EqualTo(categoryName));
Assert.That((String)responseBody.tags[0].name, Is.EqualTo(tagsName));

}

[Test, Order(3)]
public void PutPetTest()
{
    // Configura
    // Os dados de entrada vão formar o body da alteração
    // Vamos usar uma classe de modelo
    PetModel petModel = new PetModel();
    petModel.id = 3269347;
    petModel.category = new Category(1, "cat");
    petModel.name = "Serafina";
    petModel.photoUrls = new String[]{""};
    petModel.tags = new Tag[]{new Tag(1, "vacinada"), 
                              new Tag(2, "castrada")};
    petModel.status = "pending";

    // Tranformar o modelo acima em um arquivo json
    String jsonBody = JsonConvert.SerializeObject(petModel, Formatting.Indented);
    Console.WriteLine(jsonBody);

    var client = new RestClient(BASE_URL);
    var request = new RestRequest("pet", Method.Put);
    request.AddBody(jsonBody);


    // Executa
    var response = client.Execute(request);

    // Valida
    var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
    Console.WriteLine(responseBody);

    Assert.That((int)response.StatusCode, Is.EqualTo(200));
    Assert.That((int)responseBody.Id, Is.EqualTo(petModel.id));
    Assert.That((String)responseBody.tags[1].name, Is.EqualTo(petModel.tags[1].name));
    Assert.That((String)responseBody.status, Is.EqualTo(petModel.status));

}
[Test, Order(4)]
public void DeletePetTest()

{
// Configura
String petId = Environment.GetEnvironmentVariable("petId");

var client = new RestClient(BASE_URL);
var request = new RestRequest($"pet/{petId}", Method.Delete);

// Executa
var response = client.Execute(request);

// Valida
var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

Assert.That((int)response.StatusCode, Is.EqualTo(200));
Assert.That((int)responseBody.code, Is.EqualTo(200));
Assert.That((String)responseBody.message, Is.EqualTo(petId));

}

[TestCaseSource("geTestData", new object[]{}), Order(5)]
public void PostPetDDTest(int petId,
                          int categoryId,
                          String categoryName,
                          String petName,
                          String photoUrls,
                          String tagsIds,
                          String tagsName,
                          String status
                          )                         
{
// Configura
PetModel petModel = new PetModel();
petModel.id = petId;
petModel.category = new Category(categoryId, categoryName);
petModel.name = petName;
petModel.photoUrls = new String[]{photoUrls};

// código para gerar as multiplas tags que o pet pode ter
String[] tagsIdList = tagsIds.Split(";");
String[] tagsNameList = tagsName.Split(";");

for (int i = 0; i < tagsIdsList.Length; i++)
{
    
}


petModel.tags = new Tag[]{new Tag(1, "vacinada"), 
                              new Tag(2, "castrada")};
petModel.status = status;


// Instancia o objeto do tipo RestClient com o endereço da API
var client = new RestClient(BASE_URL);

// instancia o objeto do tipo RestRequest com o complemento de endereço
// como "pet" e configurando o metodo para ser um post (inclusao)
var request = new RestRequest("pet", Method.Post);

// armazena o conteudo do arquivo pet.json na memoria
String jsonBody = File.ReadAllText(@"C:\ITERASYS\PetStore139\fixtures\Pets.csv"); 

// adiciona na requisicao o conteudo do arquivo pet1.json
request.AddBody(jsonBody);

// Executa
// executa a requisição conforme a configuração realizada
// guarda o json retornando no objeto response
var response = client.Execute(request);

// Valida
var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

// Exibe o responseBody no console
Console.WriteLine(responseBody);


// Valide que na resposta, o status code é igual ao resultado esperado (200)
Assert.That((int)response.StatusCode, Is.EqualTo(200));

// Valida o petId 
// int petId = responseBody.id;
Assert.That(petId, Is.EqualTo(3269347));

// Valida o nome do animal na resposta
String name = responseBody.name.ToString();
Assert.That(name, Is.EqualTo("Serafina"));

// Valida o status do animal na resposta
// String status = responseBody.status;
Assert.That(status, Is.EqualTo("available"));

// Armazenar os dados obtidos para usar nos proximos testes
Environment.SetEnvironmentVariable("petId", petId.ToString()); 

}

}
 