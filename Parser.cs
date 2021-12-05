using System;
using System.Collections.Generic;
class Parser {
    List<Token> tokens;
    int tokIdx;
    int nextTokIdx;
    Token currentTok;
    Token nextTok;

    public Parser(List<Token> toks)
    {
        tokens = toks;
        UpdateCurrentTok();
    }
    public void UpdateCurrentTok()
    {
        if (tokIdx >= 0 && tokIdx < tokens.Count) currentTok = tokens[tokIdx];
        if (nextTokIdx >= 0 && nextTokIdx < tokens.Count) nextTok = tokens[nextTokIdx];
    }

    public void Advance()
    {
        tokIdx += 1;
        nextTokIdx += 1;
        UpdateCurrentTok();
    }

    public Token Reverse(int amount = 1)
    {
        tokIdx -= amount;
        UpdateCurrentTok();
        return currentTok;
    }

    public TemplateToken ParseCue()
    {
        var val = ParseStatement();
        if (currentTok.tokType != Token.TT_END)
        {
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Unexpected EOF");
        }
        return val;
    }

    public TemplateToken ParseStatement()
    {
        if(currentTok.Matches("KEYWORD", "for")) return ParseForStmnt();
        else if(currentTok.Matches("KEYWORD", "endfor")) return ParseEndForStmnt();
        else if(currentTok.Matches("KEYWORD", "if")) return ParseIfStmnt();
        else if(currentTok.Matches("KEYWORD", "elif")) return ParseElifStmnt();
        else if(currentTok.Matches("KEYWORD", "else")) return ParseElseStmnt();
        else if(currentTok.Matches("KEYWORD", "endif")) return ParseEndIfStmnt();
        else throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"for\", \"if\", \"elif\", \"else\", \"endif\", or \"endfor\" keyword");
    }

    public TemplateToken ParseIfStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "if"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"if\" keyword");
        this.Advance();
        var cond = Parse();
        return new IfCue(cond, posStart, currentTok.posEnd.Copy());
    }

    public TemplateToken ParseElifStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "elif"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"elif\" keyword");
        this.Advance();
        var cond = Parse();
        return new ElifCue(cond, posStart, currentTok.posEnd.Copy());
    }

    public TemplateToken ParseElseStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "else"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"else\" keyword");
        this.Advance();
        return new TemplateToken(TemplateTokenType.ElseCue, posStart, currentTok.posEnd);
    }

    public TemplateToken ParseEndIfStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "endif"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"endfor\" keyword");
        this.Advance();
        return new TemplateToken(TemplateTokenType.EndIfCue, posStart, currentTok.posEnd);
    }

    public TemplateToken ParseForStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "for"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"for\" keyword");
        this.Advance();
        if(currentTok.tokType != "IDENT")
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected an identifier");
        var ident = currentTok;
        this.Advance();
        if(!(currentTok.Matches("KEYWORD", "in"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"in\" keyword");
        this.Advance();
        var iteratorNode = Parse();
        return new ForCue(ident, iteratorNode, posStart, currentTok.posEnd);
    }

    public TemplateToken ParseEndForStmnt()
    {
        var posStart = currentTok.posStart.Copy();
        if(!(currentTok.Matches("KEYWORD", "endfor"))) 
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected \"endfor\" keyword");
        this.Advance();
        return new TemplateToken(TemplateTokenType.EndForCue, posStart, currentTok.posEnd);
    }

    public Node Parse()
    {
        var val = ParsePipedExpression();
        if (currentTok.tokType != Token.TT_END)
        {
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Unexpected EOF");
        }
        return val;
    }

    public Node BinOp(Func<Node> fun_a, HashSet<string> ops, Func<Node> fun_b){
        var left = fun_a();
        var posStart = currentTok.posStart.Copy();
        while (ops.Contains(currentTok.tokType))
        {
            var opTok = currentTok;
            this.Advance();
            var right = fun_b();
            left = new BinOpNode(left, opTok, right, posStart, currentTok.posEnd.Copy());
        }
        return left;
    }

    public Node ParsePipedExpression()
    {
        return BinOp(this.ParseLogicExpr, new HashSet<string>{Token.TT_PIPE}, this.ParseLogicExpr);
    }

    public Node ParseLogicExpr()
    {
        return BinOp(this.ParseCompExpr, new HashSet<string>{Token.TT_AND, Token.TT_OR}, this.ParseCompExpr);
    }

    public Node ParseCompExpr()
    {
        return BinOp(this.ParseExpression, new HashSet<string>{
            Token.TT_EE,
            Token.TT_NE,
            Token.TT_GT,
            Token.TT_LT,
            Token.TT_LTE,
            Token.TT_GTE,
        }, this.ParseExpression);
    }

    public Node ParseExpression()
    {
        return BinOp(this.ParseArithExpr, new HashSet<string>{Token.TT_MINUS, Token.TT_PLUS}, this.ParseArithExpr);
    }

    public Node ParseArithExpr()
    {
        return BinOp(this.ParseTerm, new HashSet<string>{Token.TT_MUL, Token.TT_DIV}, this.ParseTerm);
    }

    public Node ParseTerm()
    {
        return BinOp(this.ParseAtom, new HashSet<string>{Token.TT_MUL, Token.TT_DIV}, this.ParseAtom);
    }

    public Node ParseAtom()
    {
        var posStart = currentTok.posStart.Copy();
        if (currentTok.tokType == Token.TT_NUMBER)
        {
            var num = currentTok;
            this.Advance();
            return new NumNode(Double.Parse(num.value), posStart, currentTok.posEnd.Copy());
        } else if (currentTok.tokType == Token.TT_RPAREN) {
            this.Advance();
            var node = this.ParsePipedExpression();
            if (currentTok.tokType != Token.TT_LPAREN) throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected a ) token");
            this.Advance();
            return node;
        } else if (currentTok.tokType == Token.TT_IDENT)
        {
            var ident = currentTok;
            this.Advance();
            return new VarAccessNode(ident.value, posStart, currentTok.posEnd.Copy());
        } else if (currentTok.tokType == Token.TT_STRING)
        {
            var str = currentTok;
            this.Advance();
            return new StrNode(str.value, posStart, currentTok.posEnd.Copy());
        } else if (currentTok.Matches("KEYWORD", "true") || currentTok.Matches("KEYWORD", "false"))
        {
            var boolTok = currentTok;
            this.Advance();
            return new BoolNode(boolTok.value == "true", posStart, currentTok.posEnd.Copy());
        } else
        {
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected a NUMBER token");
        }
    }
}