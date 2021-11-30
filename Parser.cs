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
        return BinOp(this.ParseExpression, new HashSet<string>{Token.TT_PIPE}, this.ParseExpression);
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
        } else
        {
            throw new InvalidSyntaxError(currentTok.posStart.Copy(), currentTok.posEnd.Copy(), "Expected a NUMBER token");
        }
    }
}