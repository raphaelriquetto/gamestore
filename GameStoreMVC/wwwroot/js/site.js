// mensagem rápida no canto da tela
function mostraMensagem(msg, erro) {
    var toast = $('#toast');
    toast.text(msg).toggleClass('erro', erro === true).addClass('visivel');
    clearTimeout(window.timerToast);
    window.timerToast = setTimeout(function () { toast.removeClass('visivel'); }, 3000);
}

// confirmação antes de excluir um registro dos CRUDs
function apagarRegistro(controller, id) {
    if (confirm('Confirma a exclusão do registro?'))
        location.href = '/' + controller + '/Delete?id=' + id;
}

// pré-visualização da imagem escolhida no cadastro de jogos
function exibirImagem() {
    var oFReader = new FileReader();
    oFReader.readAsDataURL(document.getElementById("Imagem").files[0]);
    oFReader.onload = function (oFREvent) {
        document.getElementById("imgPreview").src = oFREvent.target.result;
    };
}

// AJAX 1: filtra a vitrine da loja
function filtraVitrine() {
    clearTimeout(window.timerVitrine);
    // espera o usuário parar de digitar para não fazer uma chamada por tecla
    window.timerVitrine = setTimeout(function () {
        $.ajax({
            url: '/home/FiltraVitrine',
            data: {
                nome: $('#filtroNome').val(),
                categoriaId: $('#filtroCategoria').val(),
                plataformaId: $('#filtroPlataforma').val()
            },
            success: function (dados) {
                if (dados.erro != undefined)
                    mostraMensagem(dados.msg, true);
                else
                    $('#vitrine').html(dados);
            }
        });
    }, 300);
}

function limpaFiltroVitrine() {
    $('#filtroNome').val('');
    $('#filtroCategoria').val('0');
    $('#filtroPlataforma').val('0');
    filtraVitrine();
}

// AJAX 2: adiciona jogo no carrinho sem sair da página
function adicionarCarrinho(jogoId, quantidade, somar) {
    $.ajax({
        url: '/carrinho/AdicionarCarrinho',
        type: 'POST',
        data: { jogoId: jogoId, quantidade: quantidade, somar: somar },
        success: function (dados) {
            mostraMensagem(dados.msg, dados.erro);
            if (!dados.erro)
                $('#qtdeCarrinho').text(dados.qtdeItens);
        }
    });
}

// altera a quantidade dentro da tela do carrinho
function alterarQtdeCarrinho(jogoId, campo) {
    $.ajax({
        url: '/carrinho/AdicionarCarrinho',
        type: 'POST',
        data: { jogoId: jogoId, quantidade: campo.value, somar: false },
        success: function (dados) {
            if (dados.erro) {
                mostraMensagem(dados.msg, true);
                campo.value = campo.defaultValue;
                return;
            }
            if (campo.value <= 0) {
                location.reload();
                return;
            }
            campo.defaultValue = campo.value;
            $('#subtotal_' + jogoId).text(dados.subtotal);
            $('#totalCarrinho').text(dados.total);
            $('#qtdeCarrinho').text(dados.qtdeItens);
        }
    });
}

// AJAX 3: consulta de CEP na API do ViaCEP
function buscaCEP() {
    var cep = document.getElementById("Cep").value.replace('-', '').trim();
    if (cep.length != 8)
        return;

    $.ajax({
        url: 'https://viacep.com.br/ws/' + cep + '/json/',
        beforeSend: function () {
            $('#Logradouro, #Bairro, #Cidade, #Uf').val('...');
        },
        success: function (dados) {
            if (dados.erro != undefined) {
                mostraMensagem('CEP não localizado...', true);
                $('#Logradouro, #Bairro, #Cidade, #Uf').val('');
            }
            else {
                $('#Logradouro').val(dados.logradouro);
                $('#Bairro').val(dados.bairro);
                $('#Cidade').val(dados.localidade);
                $('#Uf').val(dados.uf);
                $('#Numero').focus();
            }
        },
        error: function () {
            mostraMensagem('Não foi possível consultar o CEP.', true);
            $('#Logradouro, #Bairro, #Cidade, #Uf').val('');
        }
    });
}

// AJAX 4: consulta avançada de pedidos
function aplicaFiltroConsultaPedidos() {
    $.ajax({
        url: '/pedido/ObtemDadosConsultaPedidos',
        data: {
            nomeCliente: $('#nomeCliente').val(),
            status: $('#status').val(),
            dataInicial: $('#dataInicial').val(),
            dataFinal: $('#dataFinal').val()
        },
        success: function (dados) {
            if (dados.erro != undefined)
                alert(dados.msg);
            else
                $('#resultadoConsulta').html(dados);
        }
    });
}

// AJAX 5: consulta avançada de jogos vendidos
function aplicaFiltroConsultaVendas() {
    $.ajax({
        url: '/pedido/ObtemDadosConsultaVendas',
        data: {
            nomeJogo: $('#nomeJogo').val(),
            categoriaId: $('#categoriaId').val(),
            plataformaId: $('#plataformaId').val(),
            dataInicial: $('#dataInicial').val(),
            dataFinal: $('#dataFinal').val()
        },
        success: function (dados) {
            if (dados.erro != undefined)
                alert(dados.msg);
            else
                $('#resultadoConsulta').html(dados);
        }
    });
}

// AJAX 6: altera o status do pedido
function alterarStatusPedido(id) {
    $.ajax({
        url: '/pedido/AlterarStatus',
        type: 'POST',
        data: { id: id, status: $('#statusPedido').val() },
        success: function (dados) {
            mostraMensagem(dados.msg, dados.erro);
        }
    });
}

// AJAX 7: home do admin consumindo as APIs
function formataMoeda(valor) {
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

function montaTabela(colunas, linhas) {
    var tabela = $('<table class="table table-sm mb-0">');
    var cabecalho = $('<tr>');
    colunas.forEach(function (c) { cabecalho.append($('<th>').text(c)); });
    tabela.append(cabecalho);
    linhas.forEach(function (l) {
        var tr = $('<tr>');
        l.forEach(function (v) { tr.append($('<td>').text(v)); });
        tabela.append(tr);
    });
    return tabela;
}

function carregaDashboard() {
    $.ajax({
        url: '/api/relatorios/resumo',
        success: function (r) {
            $('#resTotalPedidos').text(r.totalPedidos);
            $('#resFaturamento').text(formataMoeda(r.faturamento));
            $('#resTicketMedio').text(formataMoeda(r.ticketMedio));
            $('#resTotalClientes').text(r.totalClientes);
            $('#resTotalJogos').text(r.totalJogos);
        }
    });

    $.ajax({
        url: '/api/relatorios/vendas-por-categoria',
        success: function (lista) {
            if (lista.length == 0) { $('#vendasCategoria').text('Nenhuma venda ainda.'); return; }
            var maior = Math.max.apply(null, lista.map(function (v) { return v.valorTotal; }));
            var div = $('<div>');
            lista.forEach(function (v) {
                var perc = maior > 0 ? (v.valorTotal / maior * 100) : 0;
                div.append(
                    $('<div class="mb-2">').append(
                        $('<div class="d-flex justify-content-between small">')
                            .append($('<span>').text(v.categoria + ' (' + v.qtdeVendida + ' un.)'))
                            .append($('<strong>').text(formataMoeda(v.valorTotal))),
                        $('<div class="progress" style="height: 8px">')
                            .append($('<div class="progress-bar bg-warning">').css('width', perc + '%'))
                    ));
            });
            $('#vendasCategoria').html(div);
        }
    });

    $.ajax({
        url: '/api/relatorios/jogos-mais-vendidos?top=5',
        success: function (lista) {
            if (lista.length == 0) { $('#maisVendidos').text('Nenhuma venda ainda.'); return; }
            var linhas = lista.map(function (j, i) {
                return [(i + 1) + 'º', j.jogo, j.plataforma, j.qtdeVendida, formataMoeda(j.valorTotal)];
            });
            $('#maisVendidos').html(montaTabela(['#', 'Jogo', 'Plataforma', 'Qtde', 'Total'], linhas));
        }
    });
}
